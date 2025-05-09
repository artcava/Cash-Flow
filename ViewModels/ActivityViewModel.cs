using CashFlow.Core;
using CashFlow.Interfaces;
using CashFlow.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CashFlow.ViewModels;

public class ActivityViewModel : ViewModel
{
    private readonly IUnitOfWork _unitOfWork;
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

    public ActivityViewModel(IUnitOfWork _uow)
    {
        _unitOfWork = _uow;

        AddActivityCommand = new RelayCommand(_ => AddActivity(), CanAddActivity);
        UpdateActivityCommand = new RelayCommand(_ => UpdateActivity(), CanUpdateActivity);
        DeleteActivityCommand = new RelayCommand(_ => DeleteActivity(), CanDeleteActivity);

        LoadData();
    }

    private async Task LoadData()
    {
        Activities.Clear();
        var activities = await _unitOfWork.Activities.GetAllAsync();
        foreach (var activity in activities.OrderBy(a => a.Name))
        {
            Activities.Add(activity);
        }
    }

    private async void AddActivity()
    {
        var activity = new Activity
        {
            Name = NewActivityName,
            IsIncome = NewActivityIsIncome
        };
        await _unitOfWork.Activities.AddAsync(activity);

        Activities.Add(activity);
        NewActivityName = string.Empty;
        NewActivityIsIncome = false;
    }

    private async void UpdateActivity()
    {
        if (SelectedActivity == null) return;

        await _unitOfWork.Activities.AddAsync(SelectedActivity);
    }

    private async void DeleteActivity()
    {
        if (SelectedActivity == null) return;

        await _unitOfWork.Activities.DeleteAsync(SelectedActivity.Id);

        Activities.Remove(SelectedActivity);
        SelectedActivity = null;
    }

    private bool CanAddActivity(object param) => !string.IsNullOrEmpty(NewActivityName);
    private bool CanUpdateActivity(object param) => SelectedActivity != null;
    private bool CanDeleteActivity(object param) => SelectedActivity != null;
}