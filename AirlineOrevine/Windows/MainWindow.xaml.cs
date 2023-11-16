using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Airport.Windows;
using Common.Utilities;
using Crew.Windows;
using DbContext.Database;
using DbContext.Enums;
using DbContext.Models;
using Departure.Windows;
using Document.Windows;
using Employee.Windows;
using Flight.Windows;
using Liner.Windows;
using Passenger.Windows;
using Route.Windows;
using Ticket.Windows;
using PassengerDb = DbContext.Models.Passenger;
using LinerDb = DbContext.Models.Liner;
using EmployeeDb = DbContext.Models.Employee;
using RouteDb = DbContext.Models.Route;
using AirportDb = DbContext.Models.Airport;
using CrewDb = DbContext.Models.Crew;
using TicketDb = DbContext.Models.Ticket;
using DepartureDb = DbContext.Models.Departure;
using FlightDb = DbContext.Models.Flight;
using MenuItem = System.Windows.Controls.MenuItem;

namespace AirlineOrevine.Windows;

public partial class MainWindow
{
    private readonly AirlineOrevineDbContext _dbContext;

    private const string PassengerTitle = "Пассажиры";
    private const string LinerTitle = "Авиалайнеры";
    private const string AirportTitle = "Аэропорты";
    private const string EmployeeTitle = "Сотрудники";
    private const string CrewTitle = "Экипажы";
    private const string FlightTitle = "Рейсы";
    private const string DepartureTitle = "Вылеты";
    private const string TicketTitle = "Билеты";
    private const string RouteTitle = "Маршруты";
    private const string AdminTitle = "Панель администратора";
    private const string HelpTitle = "Справка";
    private const string RecoverPasswordTitle = "Изменение пароля пользователя";
    private const string FirstDocumentTitle = "Список пассажиров";

    public List<PassengerDb> Passengers { get; set; }
    public List<LinerDb> Liners { get; set; }
    private List<AirportDb> Airports { get; set; }
    public List<AirportDb> CurrentAirports { get; set; }
    private List<EmployeeDb> Employees { get; set; }
    public List<RouteDb> Routes { get; set; }
    public List<CrewDb> Crews { get; set; }
    public List<DepartureDb> Departures { get; set; }
    public List<TicketDb> Tickets { get; set; }
    private User User { get; set; }
    public List<UserTypes> UserTypes { get; set; }

    public MainWindow(AirlineOrevineDbContext dbContext, User user)
    {
        InitializeComponent();
        _dbContext = dbContext;
        DataContext = this;
        User = user;

        TabControl.SelectedItem = HelpTabItem;
        TitleTextBox.Text = HelpTitle;

        InitMenuItems();

        InitAccesRights();

        if (User.UserType != DbContext.Enums.UserTypes.Admin)
        {
            var adminMenuItem = GetMenuItemByName("AdminMenuItem");
            adminMenuItem.Visibility = Visibility.Collapsed;
        }

        RefreshPassengerGrid();
        RefreshLinerGrid();
        RefreshAirportGrid();
        RefreshEmployeeGrid();
        RefreshRouteGrid();
        RefreshCrewGrid();
        RefreshFlightGrid();
        RefreshDepartureGrid();
        RefreshTicketGrid();
        RefreshUserGrid();
        RefreshFirstDocumentGrid();

        Closing += MainWindow_Closing;
    }

    private void InitAccesRights()
    {
        var passengerFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.PassengerForm);
        AccessRightForm(passengerFormAccessRight, GetMenuItemByName("PassengerMenuItem"), PassengerGridItem,
            AddPassengerButton,
            EditPassengerButton, DeletePassengerButton);

        var linerFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.LinerForm);
        AccessRightForm(linerFormAccessRight, GetMenuItemByName("LinerMenuItem"), LinerGridItem, AddLinerButton,
            EditLinerButton,
            DeleteLinerButton);

        var airportFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.AirportForm);
        AccessRightForm(airportFormAccessRight, GetMenuItemByName("AirportMenuItem"), AirportGridItem, AddAirportButton,
            EditAirportButton,
            DeleteAirportButton);

        var routeFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.RouteForm);
        AccessRightForm(routeFormAccessRight, GetMenuItemByName("RouteMenuItem"), RouteGridItem, AddRouteButton,
            EditRouteButton,
            DeleteRouteButton);

        var employeeFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.EmployeeForm);
        AccessRightForm(employeeFormAccessRight, GetMenuItemByName("EmployeeMenuItem"), EmployeeGridItem,
            AddEmployeeButton,
            EditEmployeeButton, DeleteEmployeeButton);

        var crewFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.CrewForm);
        AccessRightForm(crewFormAccessRight, GetMenuItemByName("CrewMenuItem"), CrewGridItem, AddCrewButton,
            EditCrewButton,
            DeleteCrewButton);

        var flightFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.FlightForm);
        AccessRightForm(flightFormAccessRight, GetMenuItemByName("FlightMenuItem"), FlightGridItem, AddFlightButton,
            EditFlightButton,
            DeleteFlightButton);

        var departureFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.DepartureForm);
        AccessRightForm(departureFormAccessRight, GetMenuItemByName("DepartureMenuItem"), DepartureGridItem,
            AddDepartureButton,
            EditDepartureButton, DeleteDepartureButton);

        var ticketFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.TicketForm);
        AccessRightForm(ticketFormAccessRight, GetMenuItemByName("TicketMenuItem"), TicketGridItem, AddTicketButton,
            EditTicketButton,
            DeleteTicketButton);

        var firstDocumentFormAccessRight = User.AccessRights.Single(x => x.Form == FormTypes.FirstDocumentForm);
        AccessRightForm(firstDocumentFormAccessRight, GetMenuItemByName("FirstDocumentMenuItem"), null, null,
            null,
            null);
    }

    private void InitMenuItems()
    {
        var menuItems = (_dbContext.MenuItems).ToList();

        var parents = menuItems.Where(x => x.ParentId == null).OrderBy(x => x.Priority);

        foreach (var parent in parents)
        {
            var parentItem = AddParentToMenuItem(parent.ItemName, parent.DllName, parent.FunctionName);

            var children = menuItems.Where(x => x.ParentId.HasValue && x.ParentId == parent.Id)
                .OrderBy(x => x.Priority);

            if (children.Any())
            {
                foreach (var child in children)
                {
                    AddChildToMenuItem(parentItem, child.ItemName, child.DllName, child.FunctionName);
                }
            }
        }
    }

    private System.Windows.Controls.MenuItem AddParentToMenuItem(string header, string name,
        string function)
    {
        System.Windows.Controls.MenuItem parentItem = new System.Windows.Controls.MenuItem
        {
            Name = name,
            Header = header,
            Cursor = Cursors.Hand
        };

        if (function is not null)
        {
            RoutedEventHandler eventHandler = (sender, e) =>
            {
                Type type = GetType();

                MethodInfo method = type.GetMethod(function);

                if (method != null)
                {
                    object[] methodParams = { sender, e };
                    method.Invoke(this, methodParams);
                }
                else
                {
                    Console.WriteLine("Метод не найден: " + function);
                }
            };

            parentItem.Click += eventHandler;
        }

        Menu.Items.Add(parentItem);

        return parentItem;
    }

    private void AddChildToMenuItem(MenuItem parent, string header, string name,
        string function)
    {
        MenuItem childItem = new MenuItem
        {
            Name = name,
            Header = header,
            Cursor = Cursors.Hand
        };

        if (function is not null)
        {
            RoutedEventHandler eventHandler = (sender, e) =>
            {
                Type type = GetType();

                MethodInfo method = type.GetMethod(function);

                if (method != null)
                {
                    object[] methodParams = { sender, e };
                    method.Invoke(this, methodParams);
                }
                else
                {
                    Console.WriteLine("Метод не найден: " + function);
                }
            };

            childItem.Click += eventHandler;
        }

        parent.Items.Add(childItem);
    }

    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        var authWindow = new AuthWindow();
        authWindow.Show();
    }

    public MenuItem GetMenuItemByName(string menuItemName)
    {
        Menu menu = Menu;

        if (menu != null)
        {
            foreach (var item in menu.Items.OfType<MenuItem>())
            {
                if (item.Name == menuItemName)
                    return item;

                MenuItem subMenuItem = FindMenuItemByName(item, menuItemName);
                if (subMenuItem != null)
                    return subMenuItem;
            }
        }

        return null;
    }

    private MenuItem FindMenuItemByName(MenuItem menuItem, string menuItemName)
    {
        foreach (var item in menuItem.Items.OfType<MenuItem>())
        {
            if (item.Name == menuItemName)
                return item;

            MenuItem subMenuItem = FindMenuItemByName(item, menuItemName);
            if (subMenuItem != null)
                return subMenuItem;
        }

        return null;
    }


    private void AccessRightForm(AccessRight accessRight, MenuItem? menuItem, Grid? grid, Button? addButton,
        Button? editButton, Button? deleteButton)
    {
        if (!accessRight.Read)
        {
            if (menuItem is not null)
            {
                menuItem.Visibility = Visibility.Collapsed;
            }

            if (grid is not null)
            {
                grid.Visibility = Visibility.Collapsed;
            }

            return;
        }

        if (addButton is not null)
        {
            addButton.IsEnabled = accessRight.Write;
        }

        if (editButton is not null)
        {
            editButton.IsEnabled = accessRight.Edit;
        }

        if (deleteButton is not null)
        {
            deleteButton.IsEnabled = accessRight.Delete;
        }
    }

    private void RefreshUserGrid()
    {
        var userTypes = Enum.GetValues(typeof(UserTypes)).Cast<UserTypes>().ToList();
        UserTypeColumn.ItemsSource = userTypes;
        UserGrid.ItemsSource = _dbContext.Users.Include(x => x.AccessRights).ToList();
        UserGrid.Items.Refresh();
    }

    private void RefreshPassengerGrid()
    {
        PassengerGrid.ItemsSource = _dbContext.Passengers.ToList();
        PassengerGrid.Items.Refresh();
    }

    private void RefreshAirportGrid()
    {
        Airports = _dbContext.Airports.ToList();
        AirportGrid.ItemsSource = Airports;

        AirportGrid.Items.Refresh();
    }

    private void RefreshFlightGrid()
    {
        FlightGrid.ItemsSource = _dbContext.Flights
            .ToList();
        FlightGrid.Items.Refresh();
    }

    private void RefreshCrewGrid()
    {
        CrewGrid.ItemsSource = _dbContext.Crews
            .AsSplitQuery()
            .Include(x => x.Employees)
            .ToList();
        CrewGrid.Items.Refresh();
    }

    private void RefreshTicketGrid()
    {
        TicketGrid.ItemsSource = _dbContext.Tickets
            .AsSplitQuery()
            .Include(x => x.Departure)
            .ToList();
        TicketGrid.Items.Refresh();
        var a = TicketGrid.ItemsSource;
    }

    private void RefreshDepartureGrid()
    {
        DepartureGrid.ItemsSource = _dbContext.Departures
            .AsSplitQuery()
            .Include(x => x.Crew)
            .Include(x => x.Liner)
            .Include(x => x.Flight)
            .ToList();
        CrewGrid.Items.Refresh();
    }

    private void RefreshRouteGrid()
    {
        RouteGrid.ItemsSource = _dbContext.Routes
            .AsSplitQuery()
            .Include(x => x.StartingPoint)
            .Include(x => x.EndingPoint)
            .Include(x => x.WayPoints)
            .ToList();

        RouteGrid.Items.Refresh();
    }

    private void RefreshEmployeeGrid()
    {
        Employees = _dbContext.Employees.ToList();
        EmployeeGrid.ItemsSource = Employees;

        EmployeeGrid.Items.Refresh();
    }

    private void RefreshLinerGrid()
    {
        LinerGrid.ItemsSource = _dbContext.Liners.ToList();
        LinerGrid.Items.Refresh();
    }

    private void EditPassengerButton_Click(object sender, RoutedEventArgs e)
    {
        if (PassengerGrid.SelectedItems.Count > 0)
        {
            var editPassengerWindow =
                new EditPassengerWindow(_dbContext, (PassengerDb)PassengerGrid.SelectedItems[0]!, false);
            editPassengerWindow.ShowDialog();
            _dbContext.Entry((PassengerDb)PassengerGrid.SelectedItems[0]!).Reload();
            RefreshPassengerGrid();
        }
        else
        {
            MessageBox.Show("Выберите пассажира для редактирования");
        }
    }

    private void AddPassengerButton_Click(object sender, RoutedEventArgs e)
    {
        PassengerDb newPassenger = new PassengerDb();
        var addPassengerWindow = new EditPassengerWindow(_dbContext, newPassenger, true);
        addPassengerWindow.ShowDialog();
        RefreshPassengerGrid();
    }

    private void DeletePassengerButton_Click(object sender, RoutedEventArgs e)
    {
        if (PassengerGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < PassengerGrid.SelectedItems.Count; i++)
            {
                if (PassengerGrid.SelectedItems[i] is PassengerDb passenger)
                {
                    _dbContext.Passengers.Remove(passenger);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите пассажира для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshPassengerGrid();
    }

    private void AddLinerButton_Click(object sender, RoutedEventArgs e)
    {
        LinerDb newLiner = new LinerDb();
        var addLinerWindow = new EditLinerWindow(_dbContext, newLiner, true);
        addLinerWindow.ShowDialog();
        RefreshLinerGrid();
    }

    private void EditLinerButton_Click(object sender, RoutedEventArgs e)
    {
        if (LinerGrid.SelectedItems.Count > 0)
        {
            var editLinerWindow = new EditLinerWindow(_dbContext, (LinerDb)LinerGrid.SelectedItems[0]!, false);
            editLinerWindow.ShowDialog();
            _dbContext.Entry((LinerDb)LinerGrid.SelectedItems[0]!).Reload();
            RefreshLinerGrid();
        }
        else
        {
            MessageBox.Show("Выберите авиалайнер для редактирования");
        }
    }

    private void DeleteLinerButton_Click(object sender, RoutedEventArgs e)
    {
        if (LinerGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < LinerGrid.SelectedItems.Count; i++)
            {
                if (LinerGrid.SelectedItems[i] is LinerDb liner)
                {
                    _dbContext.Liners.Remove(liner);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите авиалайнер для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshLinerGrid();
    }


    private void SaveUserButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException exception)
        {
            MessageBox.Show(exception.Message);
        }
    }

    private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
    {
        EmployeeDb newEmployee = new EmployeeDb();
        var addEmployeeWindow = new EditEmployeeWindow(_dbContext, newEmployee, true);
        addEmployeeWindow.ShowDialog();
        RefreshEmployeeGrid();
    }

    private void EditEmployeeButton_Click(object sender, RoutedEventArgs e)
    {
        if (EmployeeGrid.SelectedItems.Count > 0)
        {
            var editEmployeeWindow =
                new EditEmployeeWindow(_dbContext, (EmployeeDb)EmployeeGrid.SelectedItems[0]!, false);
            editEmployeeWindow.ShowDialog();
            _dbContext.Entry((EmployeeDb)EmployeeGrid.SelectedItems[0]!).Reload();
            RefreshEmployeeGrid();
        }
        else
        {
            MessageBox.Show("Выберите сотрудника для редактирования");
        }
    }

    private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
    {
        if (EmployeeGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < EmployeeGrid.SelectedItems.Count; i++)
            {
                if (EmployeeGrid.SelectedItems[i] is EmployeeDb employee)
                {
                    _dbContext.Employees.Remove(employee);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите сотрудника для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshEmployeeGrid();
    }

    private void AddRouteButton_Click(object sender, RoutedEventArgs e)
    {
        RouteDb newRoute = new RouteDb();
        var editRouteWindow = new EditRouteWindow(_dbContext, newRoute, true);
        editRouteWindow.ShowDialog();
        RefreshRouteGrid();
    }

    private void EditRouteButton_Click(object sender, RoutedEventArgs e)
    {
        if (RouteGrid.SelectedItems.Count > 0)
        {
            var editRouteWindow = new EditRouteWindow(_dbContext, (RouteDb)RouteGrid.SelectedItems[0]!, false);
            editRouteWindow.ShowDialog();
            _dbContext.Entry((RouteDb)RouteGrid.SelectedItems[0]!).Reload();
            RefreshRouteGrid();
        }
        else
        {
            MessageBox.Show("Выберите маршрут для редактирования");
            return;
        }
    }

    private void DeleteRouteButton_Click(object sender, RoutedEventArgs e)
    {
        if (RouteGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < RouteGrid.SelectedItems.Count; i++)
            {
                if (RouteGrid.SelectedItems[i] is RouteDb route)
                {
                    _dbContext.Routes.Remove(route);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите маршрут для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshRouteGrid();
    }

    private void AddAirportButton_Click(object sender, RoutedEventArgs e)
    {
        AirportDb newAirport = new AirportDb();
        var editAirportWindow = new EditAirportWindow(_dbContext, newAirport, true);
        editAirportWindow.ShowDialog();
        RefreshAirportGrid();
    }

    private void EditAirportButton_Click(object sender, RoutedEventArgs e)
    {
        if (AirportGrid.SelectedItems.Count > 0)
        {
            var editAirportWindow = new EditAirportWindow(_dbContext, (AirportDb)AirportGrid.SelectedItems[0]!, false);
            editAirportWindow.ShowDialog();
            _dbContext.Entry((AirportDb)AirportGrid.SelectedItems[0]!).Reload();
            RefreshAirportGrid();
        }
        else
        {
            MessageBox.Show("Выберите аэропорт для редактирования");
        }
    }

    private void DeleteAirportButton_Click(object sender, RoutedEventArgs e)
    {
        if (AirportGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < AirportGrid.SelectedItems.Count; i++)
            {
                if (AirportGrid.SelectedItems[i] is AirportDb airport)
                {
                    _dbContext.Airports.Remove(airport);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите аэропорт для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshAirportGrid();
    }

    private void AddCrewButton_Click(object sender, RoutedEventArgs e)
    {
        CrewDb newCrew = new CrewDb();
        var editCrewWindow = new EditCrewWindow(_dbContext, newCrew, true);
        editCrewWindow.ShowDialog();
        RefreshCrewGrid();
    }

    private void EditCrewButton_Click(object sender, RoutedEventArgs e)
    {
        if (CrewGrid.SelectedItems.Count > 0)
        {
            var editCrewWindow = new EditCrewWindow(_dbContext, (CrewDb)CrewGrid.SelectedItems[0]!, false);
            editCrewWindow.ShowDialog();
            _dbContext.Entry((CrewDb)CrewGrid.SelectedItems[0]!).Reload();
            RefreshCrewGrid();
        }
        else
        {
            MessageBox.Show("Выберите экипаж для редактирования");
        }
    }

    private void DeleteCrewButton_Click(object sender, RoutedEventArgs e)
    {
        if (CrewGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < CrewGrid.SelectedItems.Count; i++)
            {
                if (CrewGrid.SelectedItems[i] is CrewDb crew)
                {
                    _dbContext.Crews.Remove(crew);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите экипаж для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshCrewGrid();
    }

    private void AddFlightButton_Click(object sender, RoutedEventArgs e)
    {
        FlightDb newFlight = new FlightDb();
        var editFlightWindow = new EditFlightWindow(_dbContext, newFlight, true);
        editFlightWindow.ShowDialog();
        RefreshFlightGrid();
    }

    private void EditFlightButton_Click(object sender, RoutedEventArgs e)
    {
        if (FlightGrid.SelectedItems.Count > 0)
        {
            var editFlightWindow = new EditFlightWindow(_dbContext, (FlightDb)FlightGrid.SelectedItems[0]!, false);
            editFlightWindow.ShowDialog();
            _dbContext.Entry((FlightDb)FlightGrid.SelectedItems[0]!).Reload();
            RefreshFlightGrid();
        }
        else
        {
            MessageBox.Show("Выберите рейс для редактирования");
        }
    }

    private void DeleteFlightButton_Click(object sender, RoutedEventArgs e)
    {
        if (FlightGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < FlightGrid.SelectedItems.Count; i++)
            {
                if (FlightGrid.SelectedItems[i] is FlightDb flight)
                {
                    _dbContext.Flights.Remove(flight);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите рейс для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshFlightGrid();
    }

    private void AddDepartureButton_Click(object sender, RoutedEventArgs e)
    {
        DepartureDb newDeparture = new DepartureDb();
        var editDepartureWindow = new EditDepartureWindow(_dbContext, newDeparture, true);
        editDepartureWindow.ShowDialog();
        RefreshDepartureGrid();
    }

    private void EditDepartureButton_Click(object sender, RoutedEventArgs e)
    {
        if (DepartureGrid.SelectedItems.Count > 0)
        {
            var editDepartureWindow =
                new EditDepartureWindow(_dbContext, (DepartureDb)DepartureGrid.SelectedItems[0]!, false);
            editDepartureWindow.ShowDialog();
            _dbContext.Entry((DepartureDb)DepartureGrid.SelectedItems[0]!).Reload();
            RefreshDepartureGrid();
        }
        else
        {
            MessageBox.Show("Выберите вылет для редактирования");
        }
    }

    private void DeleteDepartureButton_Click(object sender, RoutedEventArgs e)
    {
        if (DepartureGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < DepartureGrid.SelectedItems.Count; i++)
            {
                if (DepartureGrid.SelectedItems[i] is DepartureDb departure)
                {
                    _dbContext.Departures.Remove(departure);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите вылет для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshDepartureGrid();
    }

    private void DeleteTicketButton_Click(object sender, RoutedEventArgs e)
    {
        if (TicketGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < TicketGrid.SelectedItems.Count; i++)
            {
                if (TicketGrid.SelectedItems[i] is TicketDb ticket)
                {
                    _dbContext.Tickets.Remove(ticket);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите билет для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshTicketGrid();
    }

    private void EditTicketButton_Click(object sender, RoutedEventArgs e)
    {
        if (TicketGrid.SelectedItems.Count > 0)
        {
            var editTicketWindow = new EditTicketWindow(_dbContext, (TicketDb)TicketGrid.SelectedItems[0]!, false);
            editTicketWindow.ShowDialog();
            _dbContext.Entry((TicketDb)TicketGrid.SelectedItems[0]!).Reload();
            RefreshTicketGrid();
        }
        else
        {
            MessageBox.Show("Выберите билет для редактирования");
        }
    }

    private void AddTicketButton_Click(object sender, RoutedEventArgs e)
    {
        TicketDb newTicket = new TicketDb();
        var editTicketWindow = new EditTicketWindow(_dbContext, newTicket, true);
        editTicketWindow.ShowDialog();
        RefreshTicketGrid();
    }

    private void EditAccessRightButton_Click(object sender, RoutedEventArgs e)
    {
        if (UserGrid.SelectedItems.Count > 0)
        {
            var editAccessRightWindow = new EditAccessRightWindow(_dbContext, (User)UserGrid.SelectedItems[0]!);
            editAccessRightWindow.ShowDialog();
            _dbContext.Entry((User)UserGrid.SelectedItems[0]!).Reload();
            RefreshUserGrid();
        }
        else
        {
            MessageBox.Show("Выберите пользователя для редактирования прав");
        }
    }

    private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
    {
        if (UserGrid.SelectedItems.Count > 0)
        {
            for (int i = 0; i < UserGrid.SelectedItems.Count; i++)
            {
                if (UserGrid.SelectedItems[i] is User user)
                {
                    if (user.UserType != DbContext.Enums.UserTypes.Admin)
                    {
                        _dbContext.Users.Remove(user);
                    }
                    else
                    {
                        MessageBox.Show("Вы не можете удалить администратора");
                        return;
                    }
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите пользователя для удаления");
            return;
        }

        _dbContext.SaveChanges();
        RefreshTicketGrid();
    }

    private void RecoverPasswordButton_Click(object sender, RoutedEventArgs e)
    {
        var oldPassword = OldPasswordTexBox.Password;
        var newPassword = NewPasswordTexBox.Password;
        var confirmPassword = ConfirmPasswordTexBox.Password;

        if (oldPassword == String.Empty)
        {
            MessageBox.Show("Старый пароль не заполнен. Введите старый пароль");
            return;
        }

        if (newPassword == String.Empty)
        {
            MessageBox.Show("Новый пароль не заполнен. Введите новый пароль");
            return;
        }

        if (confirmPassword == String.Empty)
        {
            MessageBox.Show("Подтвердите новый пароль");
            return;
        }

        if (newPassword.Length < 5)
        {
            MessageBox.Show("Новый пароль не может быть меньше 5 символов");
            return;
        }

        if (newPassword != confirmPassword)
        {
            MessageBox.Show("Пароли не совпадают. Подтвердите новый пароль");
            return;
        }

        if (User.Password == PasswordEncrypter.GetHash(oldPassword))
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == User.Id);

            if (user is not null)
            {
                user.Password = PasswordEncrypter.GetHash(NewPasswordTexBox.Password);;
                _dbContext.SaveChanges();
                OldPasswordTexBox.Password = string.Empty;
                NewPasswordTexBox.Password = string.Empty;
                ConfirmPasswordTexBox.Password = string.Empty;
                MessageBox.Show("Пароль успешно изменён!");
            }
            else
            {
                MessageBox.Show("Ошибка: не удалось найти данного пользователя");
            }           

        }
        else
        {
            MessageBox.Show("Неверный старый пароль!");
        }
    }

    private void RefreshFirstDocumentGrid()
    {
        var search = SearchPassengerTextBox.Text.ToLowerInvariant();
        FirstDocumentGrid.ItemsSource = _dbContext.Passengers
            .AsEnumerable()
            .Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Patronymic.ToLower().Contains(search) ||
                x.IssuedBy.ToLower().Contains(search) ||
                x.PassportId.ToString().Contains(search) ||
                x.PassportSeries.ToString().Contains(search))
            .ToList();

        FirstDocumentGrid.Items.Refresh();
    }

    private void SearchPassengerButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshFirstDocumentGrid();
    }

    public void OpenPassengerButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = PassengerTabItem;
        TitleTextBox.Text = PassengerTitle;
    }

    public void OpenLinerButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = LinerTabItem;
        TitleTextBox.Text = LinerTitle;
    }

    public void OpenAirportButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = AirportTabItem;
        TitleTextBox.Text = AirportTitle;
    }

    public void OpenRouteButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = RouteTabItem;
        TitleTextBox.Text = RouteTitle;
    }

    public void OpenEmployeeButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = EmployeeTabItem;
        TitleTextBox.Text = EmployeeTitle;
    }

    public void OpenCrewButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = CrewTabItem;
        TitleTextBox.Text = CrewTitle;
    }

    public void OpenFlightButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = FlightTabItem;
        TitleTextBox.Text = FlightTitle;
    }

    public void OpenDepartureButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = DepartureTabItem;
        TitleTextBox.Text = DepartureTitle;
    }

    public void OpenTicketButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = TicketTabItem;
        TitleTextBox.Text = TicketTitle;
    }

    public void OpenHelpButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = HelpTabItem;
        TitleTextBox.Text = HelpTitle;
    }

    public void OpenFirstDocumentButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = FirstDocumentTabItem;
        TitleTextBox.Text = FirstDocumentTitle;
        RefreshFirstDocumentGrid();
    }

    public void OpenRecoverPasswordButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = RecoverPasswordTabItem;
        TitleTextBox.Text = RecoverPasswordTitle;
    }

    public void OpenAdminButton_Click(object sender, RoutedEventArgs e)
    {
        TabControl.SelectedItem = AdminTabItem;
        TitleTextBox.Text = AdminTitle;
    }

    public void ExportPassengerAsButton_Click(object sender, RoutedEventArgs e)
    {
        var firstDocumentWindow = new ExportFirstDocumentWindow(_dbContext, FirstDocumentGrid);
        firstDocumentWindow.ShowDialog();
    }

    public void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}