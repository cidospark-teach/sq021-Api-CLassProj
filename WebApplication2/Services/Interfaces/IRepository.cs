﻿namespace WebApplication2.Services.Interfaces
{
    public interface IRepository
    {
        Task<bool> AddAsync<T>(T entity) where T : class;
        Task<bool> UpdateAsync<T>(T entity) where T : class;
        Task<bool> DeleteAsync<T>(T entity) where T : class;
        Task<T?> GetByIdAsync<T>(string Id) where T : class;
        IQueryable<T> GetAllAsync<T>() where T : class;
    }
}
