using System;
using System.Collections.Generic;

namespace courseStaff
{
    /// <summary>
    /// Класс отдела
    /// </summary>
    [Serializable]
    public class Department
    {
        /*Поля класса данных приложения*/
        public readonly string Name;
        public List<Person> PersonList;
        /// <summary>
        /// Конструктор класса отдела 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="personList"></param>
        public Department(string name, List<Person> personList)
        {
            Name = name;
            PersonList = personList;
        }
    }
}