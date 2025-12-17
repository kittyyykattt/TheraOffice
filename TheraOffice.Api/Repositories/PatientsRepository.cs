using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TheraOffice.Api.Models;

namespace TheraOffice.Api.Repositories
{
    public static class PatientsRepository
    {
        private static readonly string _filePath =
            Path.Combine(AppContext.BaseDirectory, "patients.json");

        private static readonly object _lock = new();

        private static readonly List<Patient> _patients = LoadFromFile();

        public static IEnumerable<Patient> GetAll() => _patients;

        public static Patient? GetById(Guid id) =>
            _patients.FirstOrDefault(p => p.Id == id);

        public static void Add(Patient patient)
        {
            lock (_lock)
            {
                if (patient.Id == Guid.Empty)
                    patient.Id = Guid.NewGuid();

                _patients.Add(patient);
                SaveToFile();
            }
        }

        public static bool Update(Guid id, Patient updated)
        {
            lock (_lock)
            {
                var existing = GetById(id);
                if (existing is null) return false;

                existing.FirstName = updated.FirstName;
                existing.LastName  = updated.LastName;
                existing.Address   = updated.Address;
                existing.BirthDate = updated.BirthDate;
                existing.Race      = updated.Race;
                existing.Gender    = updated.Gender;

                SaveToFile();
                return true;
            }
        }

        public static bool Delete(Guid id)
        {
            lock (_lock)
            {
                var existing = GetById(id);
                if (existing is null) return false;

                _patients.Remove(existing);
                SaveToFile();
                return true;
            }
        }

        public static IEnumerable<Patient> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return _patients;
            }

            query = query.Trim().ToLower();

            return _patients.Where(p =>
                (!string.IsNullOrEmpty(p.FirstName) && p.FirstName.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(p.LastName)  && p.LastName.ToLower().Contains(query))
            );
        }



        private static List<Patient> LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Patient>();

                var json = File.ReadAllText(_filePath);
                var patients = JsonSerializer.Deserialize<List<Patient>>(json);

                if (patients == null)
                    return new List<Patient>();

                foreach (var p in patients)
                {
                    if (p.Id == Guid.Empty)
                        p.Id = Guid.NewGuid();
                }

                return patients;
            }
            catch
            {
                return new List<Patient>();
            }
        }

        private static void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(_patients, options);
                File.WriteAllText(_filePath, json);
            }
            catch
            {
            }
        }
    }
}
