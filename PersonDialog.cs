using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace courseStaff
{
    /// <summary>
    /// Класс диалогового окна работника
    /// </summary>
    public partial class PersonDialog : Form
    {
        /*Поле работника*/
        private readonly Person _personOld;
        /// <summary>
        /// Конструктор диалогового окна добавления работника
        /// </summary>
        public PersonDialog(Person personOld)
        {
            InitializeComponent();

            if (personOld != null)
            {
                _personOld = personOld;
                ChangeControls();
            }
            else
                comboBoxSalaryType.SelectedIndex = 0;
            
            comboBoxSalaryType.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        /// <summary>
        /// Метод изменения контроллеров
        /// </summary>
        private void ChangeControls()
        {
            textBoxName.Text = _personOld.Name;
            textBoxSurname.Text = _personOld.Surname;
            textBoxPatronymic.Text = _personOld.Patronymic;
            comboBoxSalaryType.SelectedIndex = _personOld.GetType() == typeof(JobPrice) ? 0 : 1;
            textBoxSalary.Text = _personOld.Salary.ToString(CultureInfo.CurrentCulture);
            numericUpDownExperience.Value = _personOld.HiringExperience;
            maskedTextBoxPhone.Text = _personOld.Phone;
            textBoxPosition.Text = _personOld.Position;
        }
        /// <summary>
        /// Метод проверки заполненности полей
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private void CheckDialog()
        {
            if (tableLayoutPanel.Controls.OfType<TextBox>().Any(control => control.Text == string.Empty) ||
                !double.TryParse(textBoxSalary.Text, out var salary) || salary <= 0 ||
                !maskedTextBoxPhone.MaskFull)
                throw new FormatException("Введены неверные значения, либо не все поля заполнены");
        }
        /// <summary>
        /// Метод получения добавленного работника
        /// </summary>
        /// <returns></returns>
        public Person GetPerson(string id, string departmentName)
        {
            CheckDialog();
            var name = textBoxName.Text;
            var surname = textBoxSurname.Text;
            var patronymic = textBoxPatronymic.Text;
            var typeSalary = comboBoxSalaryType.SelectedIndex;
            var salary = Convert.ToDouble(textBoxSalary.Text.Replace('.', ','));
            var phone = maskedTextBoxPhone.Text;
            var experience = Convert.ToInt32(numericUpDownExperience.Value);
            var position = textBoxPosition.Text;
            switch (typeSalary)
            {
                case 0:
                    return new JobPrice(name, surname, patronymic, phone, id, _personOld?.DepartmentName ?? departmentName,
                        experience, salary, _personOld?.HiringTime ?? DateTime.Now, position);
                case 1:
                    return new TimePrice(name, surname, patronymic, phone, id, _personOld?.DepartmentName ?? departmentName,
                        experience, salary, _personOld?.HiringTime ?? DateTime.Now, position);
                default:
                    throw new FormatException("Ошибка выбора типа зпрплаты");
            }
            
        }
        /// <summary>
        /// Метод измененеия выбора типа зарплаты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void comboBoxSalaryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxSalaryType.SelectedIndex)
            {
                case 0:
                    labelSalaryType.Text = "руб. за сделку";
                    break;
                case 1:
                    labelSalaryType.Text = "руб. за месяц";
                    break;
            }
        }
    }
}