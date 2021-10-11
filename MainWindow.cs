using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace courseStaff
{
    /// <summary>
    /// Класс главного окна
    /// </summary>
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Класс пользовательского дерева
        /// </summary>
        private class MyTreeView : TreeView
        {
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x203) // identified double click
                {
                    var localPos = PointToClient(Cursor.Position);
                    var hitTestInfo = HitTest(localPos);
                    if (hitTestInfo.Location == TreeViewHitTestLocations.StateImage)
                        m.Result = IntPtr.Zero;
                    else
                        base.WndProc(ref m);
                }
                else base.WndProc(ref m);
            }
        }
        /*Поля данных приложения и типа выбора*/
        private Data _data = new Data();
        private SelectMode _selectMode = SelectMode.None;
        /// <summary>
        /// Перечисление типа нажатия на окно
        /// </summary>
        private enum SelectMode
        {
            None = -1,
            Department,
            Person
        }
        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Метод переключения кнопок
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void ChangeButtonState()
        {
            switch (_selectMode)
            {
                case SelectMode.None:
                    buttonDeleteDepartment.Enabled = false;
                    buttonAddPerson.Enabled = false;
                    buttonDeletePerson.Enabled = false;
                    break;
                case SelectMode.Department:
                    buttonDeleteDepartment.Enabled = true;
                    buttonAddPerson.Enabled = true;
                    buttonDeletePerson.Enabled = false;
                    break;
                case SelectMode.Person:
                    buttonDeleteDepartment.Enabled = false;
                    buttonAddPerson.Enabled = false;
                    buttonDeletePerson.Enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_selectMode), _selectMode, null);
            }
        }
        /// <summary>
        /// Метод обновления таблицы
        /// </summary>
        private void RefreshDataGridView()
        {
            dataGridView.Rows.Clear();
            foreach (var person in _data.GetPersonListAll())
            {
                dataGridView.Rows.Add(
                    person.Name,
                    person.Surname,
                    person.Patronymic,
                    person.Phone,
                    DateTime.Now.Year - person.HiringTime.Year + person.HiringExperience,
                    person.GetSalaryString(),
                    person.Position,
                    person.DepartmentName,
                    person.Id);
            }
        }
        /// <summary>
        /// Метод обновления дерева сотрудников
        /// </summary>
        private void RefreshTreeView()
        {
            treeViewStaff.Nodes.Clear();
            foreach (var department in _data.DepartmentList)
            {
                treeViewStaff.Nodes.Add(department.Name);
                foreach (var person in department.PersonList)
                    treeViewStaff.Nodes[treeViewStaff.Nodes.Count - 1].Nodes.Add(person.GetFullName());
            }
            treeViewStaff.ExpandAll();
            _selectMode = SelectMode.None;
        }
        /// <summary>
        /// Метод обновления надписей
        /// </summary>
        private void RefreshLabels()
        {
            labelCountPersonal.Text = Convert.ToString(_data.GetPersonListAll().Count());
            labelCountDepartment.Text = Convert.ToString(_data.DepartmentList.Count);

            var experience = _data.GetAvgExperienceAll();
            labelAvgExperience.Text = Convert.ToString(experience, CultureInfo.CurrentCulture) + 
                                      (experience % 10 > 4 || experience % 10 < 1 || experience % 100 / 10 == 1 ? " лет" : 
                                          (experience % 10 > 1 ?" года" : " год"));
            labelAvgJobPrice.Text = Convert.ToString(Math.Round(_data.GetAvgSalaryAll(typeof(JobPrice)), 2), CultureInfo.CurrentCulture);
            labelAvgTimePrice.Text = Convert.ToString(Math.Round(_data.GetAvgSalaryAll(typeof(TimePrice)), 2), CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// Метод обновления контроллеров
        /// </summary>
        private void RefreshControls()
        {
            RefreshLabels();
            RefreshDataGridView();
            RefreshTreeView();
            ChangeButtonState();
        }
        /// <summary>
        /// Метод проверки названия отдела
        /// </summary>
        /// <param name="newDepartment"></param>
        /// <exception cref="ArgumentException"></exception>
        private void CheckDepartment(Department newDepartment)
        {
            if (_data.DepartmentList.Any(department => department.Name == newDepartment.Name))
                throw new FormatException("Такой отдел уже существует");
        }
        /// <summary>
        /// Метод получения отдела
        /// </summary>
        /// <param name="oldDepartment"></param>
        /// <returns></returns>
        private Department GetDepartment(Department oldDepartment = null)
        {
            var departmentDialog = new DepartmentDialog(oldDepartment);
            while (true)
            {
                try
                {
                    if (departmentDialog.ShowDialog(this) != DialogResult.OK) return null;
                    var newDepartment = departmentDialog.GetDepartment();
                    CheckDepartment(newDepartment);
                    return newDepartment;
                }
                catch (FormatException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
        /// <summary>
        /// Метод изменения данных отдела
        /// </summary>
        private void ChangeDepartment()
        {
            var indexDepartment = treeViewStaff.SelectedNode.Index;
            var oldDepartment = _data.DepartmentList[indexDepartment];
            var newDepartment = GetDepartment(oldDepartment);
            if (newDepartment == null) return;
            foreach (var person in oldDepartment.PersonList) 
                person.DepartmentName = newDepartment.Name;
            _data.DepartmentList[indexDepartment] = newDepartment;
            
            RefreshControls();
        }
        /// <summary>
        /// Метод добавления отдела
        /// </summary>
        private void AddDepartment()
        {
            var newDepartment = GetDepartment();
            if (newDepartment == null) return;
            _data.DepartmentList.Add(newDepartment);

            RefreshControls();
        }
        /// <summary>
        /// Метод удаления отдела
        /// </summary>
        private void DeleteDepartment()
        {
            var indexDepartment = treeViewStaff.SelectedNode.Index;
            var deleteDepartment = _data.DepartmentList[indexDepartment];
            _data.DepartmentList.Remove(deleteDepartment);
            if (_data.DepartmentList.Count == 0)
                _selectMode = SelectMode.None;
            
            RefreshControls();
        }
        /// <summary>
        /// Метод получения работника
        /// </summary>
        /// <param name="oldPerson"></param>
        /// <returns></returns>
        private Person GetPerson(Person oldPerson = null)
        {
            var personDialog = new PersonDialog(oldPerson);
            while (true)
            {
                try
                {
                    if (personDialog.ShowDialog(this) != DialogResult.OK) return null;
                    var newPerson = personDialog.GetPerson(oldPerson?.Id ?? (DateTime.Now.Ticks - new DateTime(2016, 1, 1).Ticks).ToString("X"),
                        treeViewStaff.SelectedNode.Text);
                    return newPerson;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
        /// <summary>
        /// Метод изменения работника
        /// </summary>
        private void ChangePerson()
        {
            var indexPerson = treeViewStaff.SelectedNode.Index;
            var indexDepartment = treeViewStaff.SelectedNode.Parent.Index;
            var oldPerson = _data.DepartmentList[indexDepartment].PersonList[indexPerson];
            var newPerson = GetPerson(oldPerson);
            if (newPerson == null) return;
            _data.DepartmentList[indexDepartment].PersonList[indexPerson] = newPerson;
            
            RefreshControls();
        }
        /// <summary>
        /// Метод добавления работника
        /// </summary>
        private void AddPerson()
        {
            var indexDepartment = treeViewStaff.SelectedNode.Index;
            var newPerson = GetPerson();
            if (newPerson == null) return;
            _data.DepartmentList[indexDepartment].PersonList.Add(newPerson);
            
            RefreshControls();
        }
        /// <summary>
        /// Метод удаления работника
        /// </summary>
        private void DeletePerson()
        {
            var indexPerson = treeViewStaff.SelectedNode.Index;
            var indexDepartment = treeViewStaff.SelectedNode.Parent.Index;
            _data.DepartmentList[indexDepartment].PersonList.RemoveAt(indexPerson);
            if (_data.DepartmentList[indexDepartment].PersonList.Count == 0)
                _selectMode = SelectMode.Department;
            
            RefreshControls();
        }
        /// <summary>
        /// Метод нажатия на кнопку "Добавить отдел"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddDepartment_Click(object sender, EventArgs e)
        {
            AddDepartment();
        }
        /// <summary>
        /// Метод нажатия на кнопку "Удалить отдел"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteDepartment_Click(object sender, EventArgs e)
        {
            DeleteDepartment();
        }
        /// <summary>
        /// Метод нажатия на кнопку "Добавить работника"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddPerson_Click(object sender, EventArgs e)
        {
            AddPerson();
        }
        /// <summary>
        /// Метод нажатия на кнопку "Удалить работника"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeletePerson_Click(object sender, EventArgs e)
        {
            DeletePerson();
        }
        /// <summary>
        /// Метод нажатия на кнопку "Загрузить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            /*Диалоговое окно загрузки файла*/
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "bin files (*.bin)|*.bin"
            };
            if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                /*Десериализация*/
                using (var fs = openFileDialog.OpenFile())
                {
                    var formatter = new BinaryFormatter();
                    _data = (Data)formatter.Deserialize(fs);
                    
                    RefreshControls();
                }
                MessageBox.Show("Состояние успешно загружено.",
                    "Загрузка состояния",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                MessageBox.Show("Произошла ошибка во время десериализации");
            }
        }
        /// <summary>
        /// Метод нажатия на кнопку "Сохранить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            /*Диалоговое окно сохранения файла*/
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "bin files (*.bin)|*.bin"
            };
            if (saveFileDialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                /*Сериализация*/
                using (var fs = saveFileDialog.OpenFile())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(fs, _data);
                }
                MessageBox.Show("Состояние успешно сохранено.",
                    "Сохранение состояния",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                MessageBox.Show("Произошла ошибка во время сериализации");
            }
        }
        /// <summary>
        /// Метод изменения выбора в дереве работников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewStaff_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Level)
            {
                case 0:
                    _selectMode = SelectMode.Department;
                    break;
                case 1:
                    _selectMode = SelectMode.Person;
                    break;
                default:
                    _selectMode = SelectMode.None;
                    break;
            }
            ChangeButtonState();
        }
        /// <summary>
        /// Метод двойного нажатия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void treeViewStaff_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (_selectMode)
            {
                case SelectMode.None:
                    break;
                case SelectMode.Department:
                    ChangeDepartment();
                    break;
                case SelectMode.Person:
                    ChangePerson();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}