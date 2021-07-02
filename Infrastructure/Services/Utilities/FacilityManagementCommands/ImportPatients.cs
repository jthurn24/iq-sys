using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQI.Intuition.Reporting.Repositories;
using IQI.Intuition.Domain.Repositories;
using RedArrow.Framework.Persistence;
using RedArrow.Framework.Logging;
using IQI.Intuition.Domain.Models;
using SnyderIS.sCore.Persistence;
using System.Text.RegularExpressions;

namespace IQI.Intuition.Infrastructure.Services.Utilities.FacilityManagementCommands
{
    public class ImportPatients : ICommand
    {
        public void Run(string[] args, 
            IStatelessDataContext dataContext,
            IUserRepository userRepository
            )
        {
            int facilityID = Convert.ToInt32(args[1]);
            string importPath = String.Join(" ", args.Skip(2).ToArray());

            var facility = dataContext.Fetch<Facility>(facilityID);
            var account = dataContext.Fetch<Account>(facility.Account.Id);

            var data = System.IO.File.ReadAllLines(importPath);

            foreach (var line in data)
            {
                var fields = line.Split(",".ToCharArray());
                string lastName = fields[0];
                string firstName = fields[1];
                string initial = fields[2];

                string floor = fields[3];
                string wing = fields[4];
                string room = fields[5];
                string admitted = fields[6];
                string dob = fields[7];
                string physician = fields[8];
                string physicianb = string.Empty;

                if (fields.Length > 9)
                {
                    physicianb = fields[9];
                }

                if (lastName != string.Empty && firstName != string.Empty)
                {

                    var exisitingPatient = dataContext.CreateQuery<Patient>()
                        .FilterBy(x => x.Room.Wing.Floor.Facility.Id == facility.Id)
                        .FetchAll().ToList()
                        .Where(x => x.GetFirstName() == firstName)
                        .Where(x => x.GetLastName() == lastName)
                        .FirstOrDefault();

                    if (exisitingPatient != null)
                    {
                        System.Console.WriteLine("Patient already exists {0} {1}", lastName, firstName);
                        continue;
                    }

                    var patient = new Patient(account);
                    patient.SetLastName(lastName);
                    patient.SetFirstName(firstName);

                    if (initial.Length > 0)
                    {
                        patient.SetMiddleInitial(initial[0].ToString());
                    }

                    DateTime birthDate;

                    if (DateTime.TryParse(dob, out birthDate))
                    {
                        patient.BirthDate = birthDate;
                    }

                    var floorEntity = dataContext.CreateQuery<Floor>()
                        .FilterBy(x => x.Facility.Id == facility.Id)
                        .FilterBy(x => x.Name.Contains(floor))
                        .FetchAll().FirstOrDefault();

                    if (floorEntity == null)
                    {
                        System.Console.WriteLine("Unable to locate floor {0}", floor);
                        continue;
                    }

                    var wingEntity = dataContext.CreateQuery<Wing>()
                        .FilterBy(x => x.Floor.Facility.Id == facility.Id)
                        .FilterBy(x => x.Floor.Id == floorEntity.Id)
                        .FilterBy(x => x.Name.Contains(wing)).FetchAll().FirstOrDefault();

                    if (wingEntity == null)
                    {
                        System.Console.WriteLine("Unable to locate wing {0}", wing);
                        continue;
                    }

                    var roomEntity = dataContext.CreateQuery<Room>()
                        .FilterBy(x => x.Wing.Id == wingEntity.Id)
                        .FilterBy(x => x.Name.Contains(room)).FetchAll().FirstOrDefault();

                    if (roomEntity == null)
                    {
                        roomEntity = new Room();
                        roomEntity.Wing = wingEntity;
                        roomEntity.Guid = Guid.NewGuid();
                        roomEntity.Name = room;
                        dataContext.Insert(roomEntity);
                        System.Console.WriteLine("Added Room {0}", room);
                    }

                    patient.Room = roomEntity;
                    patient.MDName = string.Concat(physicianb, " ", physician);

                    patient.CurrentStatus = Domain.Enumerations.PatientStatus.Admitted;
                    dataContext.Insert(patient);

                    var statusChange = new PatientStatusChange();
                    statusChange.Patient = patient;
                    statusChange.Status = Domain.Enumerations.PatientStatus.Admitted;

                    DateTime admitDate;

                    if (DateTime.TryParse(admitted, out admitDate))
                    {
                        statusChange.StatusChangedAt = admitDate;
                        dataContext.Insert(statusChange);
                    }

                    System.Console.WriteLine("Added patient {0} {1} with ID {2}", firstName, lastName, patient.Id);


                }
                else if (room != string.Empty)
                {
                    /* Just room */

                    var floorEntity = dataContext.CreateQuery<Floor>()
                      .FilterBy(x => x.Facility.Id == facility.Id)
                      .FilterBy(x => x.Name.Contains(floor))
                      .FetchAll().FirstOrDefault();

                    if (floorEntity == null)
                    {
                        System.Console.WriteLine("Unable to locate floor {0}", floor);
                        continue;
                    }

                    var wingEntity = dataContext.CreateQuery<Wing>()
                        .FilterBy(x => x.Floor.Facility.Id == facility.Id)
                        .FilterBy(x => x.Floor.Id == floorEntity.Id)
                        .FilterBy(x => x.Name.Contains(wing)).FetchAll().FirstOrDefault();

                    if (wingEntity == null)
                    {
                        System.Console.WriteLine("Unable to locate wing {0}", wing);
                        continue;
                    }

                    var roomEntity = dataContext.CreateQuery<Room>()
                        .FilterBy(x => x.Wing.Id == wingEntity.Id)
                        .FilterBy(x => x.Name.Contains(room)).FetchAll().FirstOrDefault();

                    if (roomEntity == null)
                    {
                        roomEntity = new Room();
                        roomEntity.Wing = wingEntity;
                        roomEntity.Guid = Guid.NewGuid();
                        roomEntity.Name = room;
                        dataContext.Insert(roomEntity);
                        System.Console.WriteLine("Added Room {0}", room);
                    }
                }


            }

        }
    }
}
