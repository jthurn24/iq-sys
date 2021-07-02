using System;
using System.Linq;
using System.Text;
using IQI.Intuition.Domain.Models;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Persistence.NHibernate;
using RedArrow.Framework.Logging;
using SnyderIS.sCore.Persistence;
using System.Collections.Generic;
using RedArrow.Framework.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using System.IO;
using IQI.Intuition.Reporting.Models.Dimensions;

namespace IQI.Intuition.Infrastructure.Services.Migration
{
    public class ConvertRoomCoordinates
    {

        private IStatelessDataContext _DataContext;
        private IDocumentStore _Store;


        public ConvertRoomCoordinates(IStatelessDataContext dataContext,
            IDocumentStore store)
        {
            _DataContext = dataContext;
            _Store = store;
        }


        public void Run(string[] args)
        {
            AddCoords();
            ImportMaps(args[1]);
        }

        public void ImportMaps(string path)
        {

            var command = _DataContext.CreateCommand();
            command.CommandText = "SELECT [Dimensions_FloorMap].[Name] ,[Dimensions_FloorMap].[Guid] ,[Dimensions_Account].[Guid] FROM [Dimensions_FloorMap] LEFT JOIN [Dimensions_Account] ON [Dimensions_Account].Id = [Dimensions_FloorMap].AccountId";
            command.CommandType = System.Data.CommandType.Text;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string name = reader[0].ToString();
                Guid sourceguid = Guid.Parse(reader[1].ToString());
                Guid accountguid = Guid.Parse(reader[2].ToString());

                string fp = string.Concat(path, sourceguid.ToString(), ".jpg");

                if (System.IO.File.Exists(fp))
                {
                    var i = System.Drawing.Image.FromFile(fp);

                    var fm = _Store.GetQueryable<FloorMap>()
                        .Where(x => x.Active == true
                            && x.Facility.Account.Id == accountguid
                            && x.Name == name)
                            .FirstOrDefault();

                    if (fm != null)
                    {
                        MemoryStream ms = new MemoryStream();
                        i.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);


                        var fimage = new FloorMapImage()
                        {
                            FloorMap = fm,
                            Image = ms.ToArray()
                        };

                        _Store.Save<FloorMapImage>(fimage);
                    }


                }
                else
                {
                    System.Console.WriteLine("Missing file.. {0} ", fp);
                }

            }

            reader.Close();
            
        }

        public void AddCoords()
        {
            var command = _DataContext.CreateCommand();
            command.CommandText = "SELECT [Coordinates],[Dimensions_Room].[Guid] ";
            command.CommandText += " FROM [Dimensions_FloorMapRoom] ";
            command.CommandText += "  LEFT JOIN [Dimensions_Room] ON  ";
            command.CommandText += "  [Dimensions_FloorMapRoom].RoomID = [Dimensions_Room].ID ";
            command.CommandText += "  LEFT JOIN [Dimensions_FloorMap] ON ";
            command.CommandText += "  [Dimensions_FloorMap].Id = [Dimensions_FloorMapRoom].FloorMapId ";
            command.CommandText += "  WHERE [Dimensions_FloorMap].Active = 1 ";

            command.CommandType = System.Data.CommandType.Text;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var coords = reader[0].ToString();
                var guid = Guid.Parse(reader[1].ToString());

                var room = _Store.GetQueryable<IQI.Intuition.Reporting.Models.Dimensions.FloorMapRoom>()
                    .Where(x => x.Room.Id == guid)
                    .FirstOrDefault();

                if (room != null)
                {
                    room.Coordinates = coords;
                    _Store.Save<IQI.Intuition.Reporting.Models.Dimensions.FloorMapRoom>(room);
                }



            }

            reader.Close();
        }

 
    }
}
