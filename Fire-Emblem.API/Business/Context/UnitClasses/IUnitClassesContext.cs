﻿using Fire_Emblem.API.Models.UnitClass;
using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Business.Context.UnitClasses
{
    public interface IUnitClassesContext
    {
        Task<List<UnitClass>> GetAllClasses();
        Task<UnitClass> GetClass(int? id = null, string name = null);
        Task<bool> AddNewClass(UnitClassDto unitClassDto);
        Task<bool> RemoveClassById(int id);
    }
}
