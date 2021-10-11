using System;
using System.Collections.Generic;
using System.Linq;

namespace courseStaff
{
    /// <summary>
    /// Класс данных приложения
    /// </summary>
    [Serializable]
    public class Data
    {
        /*Поля класса данных приложения*/
        public readonly List<Department> DepartmentList = new List<Department>();
        /// <summary>
        /// Метод получения списка персонала
        /// </summary>
        /// <returns></returns>
        public List<Person> GetPersonListAll()
        {
            return DepartmentList.SelectMany(department => department.PersonList).ToList();
        }
        /// <summary>
        /// Метод получения среднего показателя опыта
        /// </summary>
        /// <returns></returns>
        public int GetAvgExperienceAll()
        {
            return GetPersonListAll().Count == 0 ? 0 : Convert.ToInt32(Math.Round(GetPersonListAll().Average(person => DateTime.Now.Year - person.HiringTime.Year + person.HiringExperience)));
        }
        /// <summary>
        /// Метод получения среднего показателя зарплаты
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public double GetAvgSalaryAll(Type type)
        {
            var personListType = GetPersonListAll().Where(person => person.GetType() == type).ToList();
            return personListType.Count == 0 ? 0 : personListType.Average(person => person.Salary);
        }
    }
}