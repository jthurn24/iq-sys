using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Event;
using IQI.Intuition.Domain.Services.ChangeTracking;
using System.Web.Mvc;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using System.Reflection;
using System.Linq.Expressions;
using RedArrow.Framework.Extensions.Formatting;
using System.Collections;
using NHibernate.Action;
using NHibernate.Persister.Collection;
using NHibernate.Collection;

namespace IQI.Intuition.Infrastructure.Services.Protection
{
    public class ChangeTrackingService : IPostInsertEventListener, IPostDeleteEventListener, IPostUpdateEventListener
    {

        public void OnPostInsert(PostInsertEvent e)
        {
            try
            {
                var entity = e.Entity as ITrackChanges;

                if (entity != null)
                {
                    var defs = entity.GetChangeTrackingDefinition();
                    TrackInsert(defs, e.Entity);
                }
            }
            catch (Exception ex)
            {
                var logger = DependencyResolver.Current.GetService<RedArrow.Framework.Logging.ILog>();

                if (logger != null)
                {
                    logger.Error(ex);
                }
            }
        }

        public void OnPostDelete(PostDeleteEvent e)
        {
            try
            {
                var entity = e.Entity as ITrackChanges;

                if (entity != null)
                {
                    var defs = entity.GetChangeTrackingDefinition();
                    TrackDelete(defs, entity);
                }
            }
            catch (Exception ex)
            {
                var logger = DependencyResolver.Current.GetService<RedArrow.Framework.Logging.ILog>();

                if (logger != null)
                {
                    logger.Error(ex);
                }
            }
        }

        public void OnPostUpdate(PostUpdateEvent e)
        {

            try
            {
                var entity = e.Entity as ITrackChanges;

                if (entity != null)
                {
                    var fixedOldState = GetFixedOldState(e);
                    var defs = entity.GetChangeTrackingDefinition();

                    if (defs.RemoveExpression != null)
                    {
                        if (defs.RemoveExpression.Compile().Invoke(entity))
                        {
                            TrackDelete(defs, entity);
                            return;
                        }
                    }

                    TrackChange(defs, entity, e.State, fixedOldState, e.Persister.PropertyNames);
                }

            }
            catch (Exception ex)
            {
                var logger = DependencyResolver.Current.GetService<RedArrow.Framework.Logging.ILog>();

                if (logger != null)
                {
                    logger.Error(ex);
                }
            }
        }



        private object[] GetFixedOldState(PostUpdateEvent evt)
        {
            object[] old = evt.OldState;

            ITrackChanges dataObject = evt.Entity as ITrackChanges;
            if (dataObject != null)
            {
                #region get dirty collections (previous state)


                IList collUpdates = evt.Session.ActionQueue.GetType().GetField("collectionUpdates", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(evt.Session.ActionQueue) as IList;

                foreach (CollectionUpdateAction item in collUpdates)
                {
                    FieldInfo fi = item.GetType().BaseType.GetField("persister", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                    ICollectionPersister persister = fi.GetValue(item) as ICollectionPersister;
                    string propertyName = persister.Role.Split('.').Last();

                    fi = item.GetType().BaseType.GetField("collection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                    IPersistentCollection coll = fi.GetValue(item) as IPersistentCollection;

                    if (coll.Owner != evt.Entity) continue;

                    if (coll.IsDirty)
                    {
                        if (Array.IndexOf(evt.Persister.PropertyNames, propertyName) != -1)
                        {
                            if (coll.StoredSnapshot != null && (coll.StoredSnapshot as IList).Count > 0)
                            {
                                old[Array.IndexOf(evt.Persister.PropertyNames, propertyName)] = coll.StoredSnapshot;
                            }
                            else
                            {

                                old[Array.IndexOf(evt.Persister.PropertyNames, propertyName)] = null;
                            }
                        }
                    }
                }
                #endregion
            }

            return old;
        }

        private void TrackInsert(IChangeTrackingDefinition def, object newEntity)
        {
            var entry = new Domain.Models.AuditEntry();
            entry.AuditType = def.AuditCreateFlag;
            entry.DetailsText = string.Concat("Added ",def.EntityDescription.Compile().Invoke(newEntity));
            entry.DetailsMode = null;

            SubmitAuditEntry(entry, def, newEntity);
        }

        private void TrackDelete(IChangeTrackingDefinition def, object deletedEntity)
        {
            var entry = new Domain.Models.AuditEntry();
            entry.AuditType = def.AuditRemoveFlag;
            entry.DetailsText = string.Concat("Deleted ", def.EntityDescription.Compile().Invoke(deletedEntity));
            entry.DetailsMode = null;

            SubmitAuditEntry(entry, def, deletedEntity);
        }

        private void TrackChange(IChangeTrackingDefinition def, object entity, 
            object[] newState, object[] oldState, string[] properties)
        {

            var changes = new List<KeyValuePair<string, string>>();

            for (int index = 0; index < properties.Length; index++)
            {
                var name = properties[index];

                Expression<Func<object, string>> exp = x => DefaultPropertyEval(x);

                if (def.ComparisonOverrides.ContainsKey(name))
                {
                    exp = def.ComparisonOverrides[name];
                }



                var oldValue = exp.Compile().Invoke(oldState[index]);
                var newValue = exp.Compile().Invoke(newState[index]);

                if (oldValue != newValue)
                {
                    var kv = new KeyValuePair<string, string>(name, newValue);
                    changes.Add(kv);
                }
            }

            if (changes.Count() > 0)
            {
                var entry = new Domain.Models.AuditEntry();
                entry.AuditType = def.AuditEditFlag;


                var change = new ChangeData();
                change.Description = string.Concat("Modified ",def.EntityDescription.Compile().Invoke(entity));
                change.Fields = new List<ChangeData.Field>();

                foreach (var c in changes)
                {
                    var field = new ChangeData.Field();
                    field.Name = c.Key.SplitPascalCase();
                    field.Change = c.Value;
                    change.Fields.Add(field);
                }


                entry.DetailsMode = Domain.Models.AuditEntry.DETAILS_MODE_SERIALIZED_CHANGES;

                entry.DetailsText = change.Serialize();

                SubmitAuditEntry(entry, def, entity);

            }

        }


        private void SubmitAuditEntry(Domain.Models.AuditEntry entry, IChangeTrackingDefinition def, object src)
        {

            if (def.AuditTargetPatient != null)
            {
                entry.TargetPatient = def.AuditTargetPatient.Compile().Invoke(src);
            }

            if (def.AuditTargetComponent != null)
            {
                entry.TargetComponent = def.AuditTargetComponent.Compile().Invoke(src);
            }

            var actionContext = DependencyResolver.Current.GetService<IActionContext>();
            entry.PerformedAt = DateTime.Now;
            entry.Facility = actionContext.CurrentFacility;

            if (actionContext.CurrentSystemUser != null)
            {
                entry.PerformedBy = actionContext.CurrentSystemUser.Guid.ToString();
            }
            if (actionContext.CurrentUser != null)
            {
                entry.PerformedBy = actionContext.CurrentUser.Guid.ToString();
                entry.IPAddress = actionContext.CurrentUser.LastIpAddress;
            }

            /* Since we are dealing within the session flush stage of the statefull context session, we open our own stateless connection to do this work */


            var activator = DependencyResolver.Current.GetService<RedArrow.Framework.Persistence.NHibernate.ISessionActivator>();

            

            var session = activator.CreateSession(null);

            if (session.IsConnected == false)
            {
                session.Reconnect();
            }

            var t = session.BeginTransaction();
            session.Save(entry);
            t.Commit();


            //var t = dataContext.BeginTransaction();
            //dataContext.Insert(entry);
            //t.Commit();
            //dataContext.Dispose();

        }

        public static string DefaultPropertyEval(object src)
        {
            if (src == null)
            {
                return string.Empty;
            }

            var type = src.GetType();

            if (type == typeof(string))
            {
                return (string)src;
            }

            if (type == typeof(int))
            {
                return ((int)src).ToString();
            }

            if (type.IsEnum)
            {
                return System.Enum.GetName(type, src).SplitPascalCase();
            }

            if (type == typeof(DateTime))
            {
                return ((DateTime)src).FormatAsShortDate();
            }

            if (type == typeof(DateTime?))
            {
                return ((DateTime?)src).FormatAsShortDate();
            }

            if (type.GetProperties().Count(x => x.Name == "Name") > 0)
            {
                return type.GetProperties().Where(x => x.Name == "Name").First().GetValue(src, null).ToString();
            }

            if (type.GetProperties().Count(x => x.Name == "Description") > 0)
            {
                return type.GetProperties().Where(x => x.Name == "Description").First().GetValue(src, null).ToString();
            }

            return src.ToString();
        }



    }
}
