using CashFlow.Core;
using CashFlow.Data;
using CashFlow.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CashFlow.ViewModels;

public class ActivityViewModel : ViewModel
{
    private readonly CashFlowContext _context;
    private string _newActivityName = string.Empty;
    private bool _newActivityIsIncome;
    private Activity? _selectedActivity;

    private ObservableCollection<Activity> _activities = new ObservableCollection<Activity>();
    public ObservableCollection<Activity> Activities
    {
        get => _activities;
        private set
        {
            _activities = value;
            OnPropertyChanged();
        }
    }

    public string NewActivityName
    {
        get => _newActivityName;
        set
        {
            _newActivityName = value;
            OnPropertyChanged();
        }
    }

    public bool NewActivityIsIncome
    {
        get => _newActivityIsIncome;
        set
        {
            _newActivityIsIncome = value;
            OnPropertyChanged();
        }
    }

    public Activity? SelectedActivity
    {
        get => _selectedActivity;
        set
        {
            _selectedActivity = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddActivityCommand { get; }
    public ICommand UpdateActivityCommand { get; }
    public ICommand DeleteActivityCommand { get; }

    public ActivityViewModel(CashFlowContext context)
    {
        _context = context;

        AddActivityCommand = new RelayCommand(_ => AddActivity(), CanAddActivity);
        UpdateActivityCommand = new RelayCommand(_ => UpdateActivity(), CanUpdateActivity);
        DeleteActivityCommand = new RelayCommand(_ => DeleteActivity(), CanDeleteActivity);

        LoadData();
    }

    private void LoadData()
    {
        Activities.Clear();
        var activities = _context.Activities.AsNoTracking().OrderBy(a => a.Name).ToList();
        foreach (var activity in activities)
        {
            Activities.Add(activity);
        }
    }

    private void AddActivity()
    {
        var activity = new Activity
        {
            Name = NewActivityName,
            IsIncome = NewActivityIsIncome
        };
        _context.Activities.Add(activity);
        _context.SaveChanges();

        Activities.Add(activity);
        NewActivityName = string.Empty;
        NewActivityIsIncome = false;
    }

    private void UpdateActivity()
    {
        if (SelectedActivity == null) return;

        _context.Activities.Update(SelectedActivity);
        _context.SaveChanges();
    }

    private void DeleteActivity()
    {
        if (SelectedActivity == null) return;

        _context.Activities.Remove(SelectedActivity);
        _context.SaveChanges();

        Activities.Remove(SelectedActivity);
        SelectedActivity = null;
    }

    private bool CanAddActivity(object param) => !string.IsNullOrEmpty(NewActivityName);
    private bool CanUpdateActivity(object param) => SelectedActivity != null;
    private bool CanDeleteActivity(object param) => SelectedActivity != null;
}