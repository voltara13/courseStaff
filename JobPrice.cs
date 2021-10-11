using System;

namespace courseStaff
{
    /// <summary>
    /// Дочерний класс работника - работник со сдельной зарплатой
    /// </summary>
    [Serializable]
    public class JobPrice : Person
    {
        /// <summary>
        /// Конструктор работника со сдельной зарплатой
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="patronymic"></param>
        /// <param name="phone"></param>
        /// <param name="id"></param>
        /// <param name="departmentName"></param>
        /// <param name="hiringExperience"></param>
        /// <param name="salary"></param>
        /// <param name="hiringTime"></param>
        /// <param name="position"></param>
        public JobPrice(string name, string surname, string patronymic, string phone, string id, string departmentName,
            int hiringExperience, double salary, DateTime hiringTime, string position) : 
            base(name, surname, patronymic, phone, id, departmentName, hiringExperience, salary, hiringTime, position) {}
        /// <summary>
        /// Переопределенный метод получения строки зарплаты
        /// </summary>
        /// <returns></returns>
        public override string GetSalaryString()
        {
            return Salary + " руб. за сделку";
        }
    }
}