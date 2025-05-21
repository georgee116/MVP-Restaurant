// Restaurant.Services/Services/MeniuService.cs (Implementare corectată)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.DataAccess.Interfaces;
using Restaurant.DataAccess.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Services.Services
{
    public class MeniuService
    {
        private readonly IMeniuRepository _repo;

        // Folosim constructor cu instanțierea repository-ului
        public MeniuService()
        {
            _repo = new MeniuRepository();
        }

        // Constructor pentru testare cu dependency injection
        public MeniuService(IMeniuRepository repository)
        {
            _repo = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEnumerable<Meniu>> GetAllAsync()
        {
            try
            {
                return await _repo.GetAllAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MeniuService.GetAllAsync: {ex.Message}");
                throw; // Re-throw pentru a permite gestionarea în nivelul superior
            }
        }

        public async Task<Meniu?> GetByIdAsync(int id)
        {
            try
            {
                return await _repo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MeniuService.GetByIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task AddAsync(Meniu meniu)
        {
            if (meniu == null)
            {
                throw new ArgumentNullException(nameof(meniu));
            }

            try
            {
                // Verificare date
                if (string.IsNullOrWhiteSpace(meniu.Denumire))
                {
                    throw new ArgumentException("Denumirea meniului este obligatorie");
                }

                if (meniu.CategorieId <= 0)
                {
                    throw new ArgumentException("Categoria meniului este obligatorie");
                }

                // Adăugare în baza de date
                await _repo.AddAsync(meniu);
                System.Diagnostics.Debug.WriteLine($"Meniu added successfully: {meniu.Denumire}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MeniuService.AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(Meniu meniu)
        {
            if (meniu == null)
            {
                throw new ArgumentNullException(nameof(meniu));
            }

            try
            {
                // Verificare date
                if (string.IsNullOrWhiteSpace(meniu.Denumire))
                {
                    throw new ArgumentException("Denumirea meniului este obligatorie");
                }

                if (meniu.CategorieId <= 0)
                {
                    throw new ArgumentException("Categoria meniului este obligatorie");
                }

                // Actualizare în baza de date
                await _repo.UpdateAsync(meniu);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MeniuService.UpdateAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID-ul meniului trebuie să fie pozitiv", nameof(id));
            }

            try
            {
                await _repo.RemoveAsync(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MeniuService.DeleteAsync: {ex.Message}");
                throw;
            }
        }
    }
}