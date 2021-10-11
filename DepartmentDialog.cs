using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace courseStaff
{
    /// <summary>
    /// Класс диалогового окна отдела
    /// </summary>
    public partial class DepartmentDialog : Form
    {
        /*Поле отдела*/
        private readonly Department _departmentOld;
        /// <summary>
        /// Конструктор диалогового окна отдела
        /// </summary>
        public DepartmentDialog(Department departmentOld)
        {
            InitializeComponent();
            
            if (departmentOld != null)
            {
                _departmentOld = departmentOld;
                ChangeControls();
            }
        }
        /// <summary>
        /// Метод изменения контроллеров
        /// </summary>
        private void ChangeControls()
        {
            textBoxName.Text = _departmentOld.Name;
        }
        /// <summary>
        /// Метод проверки заполненности полей
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private void CheckDialog()
        {
            if (textBoxName.Text == string.Empty)
                throw new FormatException("Поле не заполнено");
        }
        /// <summary>
        /// Метод получения добавленного отдела
        /// </summary>
        /// <returns></returns>
        public Department GetDepartment()
        {
            CheckDialog();
            return new Department(textBoxName.Text, _departmentOld?.PersonList ?? new List<Person>());
        }
    }
}