using System;

namespace courseStaff
{
    [Serializable]
    /// <summary>
    /// Родительский абстрактный класс работника
    /// </summary>
    public abstract class Person
    {
        /*Поля данных работника*/
        public readonly string Name;
        public readonly string Surname;
        public readonly string Patronymic;
        public readonly string Phone;
        public readonly string Id;
        public string DepartmentName;
        public readonly int HiringExperience;
        public readonly double Salary;
        public readonly DateTime HiringTime;
        public readonly string Position;

        protected Person(string name, string surname, string patronymic, string phone, string id, string departmentName,
            int hiringExperience, double salary, DateTime hiringTime, string position)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            Phone = phone;
            Id = id;
            DepartmentName = departmentName;
            HiringExperience = hiringExperience;
            Salary = salary;
            HiringTime = hiringTime;
            Position = position;
        }
        /// <summary>
        /// Абстрактный метод получения зарплаты
        /// </summary>
        /// <returns></returns>
        public abstract string GetSalaryString();
        /// <summary>
        /// Метод получения ИФО в виде строки
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
        {
            return Name + " " + Surname + " " + Patronymic;
        }
    }
}