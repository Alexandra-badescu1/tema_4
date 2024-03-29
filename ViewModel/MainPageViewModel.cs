using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TODOlist.Models;
using TODOlist.Service;
using TODOlist.View;

namespace TODOlist.ViewModel;

 public partial class MainPageViewModel : BaseViewModel , IQueryAttributable
{
    [ObservableProperty]
    List<ToDoModel> todolist;

    [ObservableProperty]
    ToDoModel todo;

    [ObservableProperty]
    ToDoModel toSaveOnDB;

    [ObservableProperty]
    ToDoModel toDeleteOnDB;

    private readonly DbConection _dbConnection;
    public MainPageViewModel(DbConection dbConnection)
    {
        _dbConnection = dbConnection;
        //Title = "ToDoList Page";
        Todolist = new List<ToDoModel>();
        toSaveOnDB = new ToDoModel();
        todo = new ToDoModel();// #added - instantiat mobelul 
        GetInitalData();
    }

    [RelayCommand]
    private async void GetInitalData() => Todolist = await _dbConnection.GetItemsAsync();

    [RelayCommand]
    async Task GoToEditPage(ToDoModel toDo)
    {

        Todo.Id = -2;
        var navigationParameter = new Dictionary<string, object>
        {
            { "Todo", toDo }
        };

        toDo = null;
        await Shell.Current.GoToAsync(nameof(Edit),navigationParameter);
    }

    [RelayCommand]
    private async void GoToAddPage()
    {
        
        var navigationParameter = new Dictionary<string, object>
        {
            { "Todo", Todo }
        };
        Todo.Id = -1;
        await Shell.Current.GoToAsync($"{nameof(Add)}", navigationParameter);
    }

    [RelayCommand]
    private async void SaveOnDb()
    {
        await _dbConnection.SaveItemAsync(toSaveOnDB);
        Todolist = await _dbConnection.GetItemsAsync();
    }

    [RelayCommand]

    public async Task DeleteOnDb(ToDoModel todo)
    {
        Todolist.Remove(todo);
       await _dbConnection.DeleteItemAsync(todo);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    { 
        if(Todo.Id == -2 && query.ContainsKey("IdUser"))
        {
                if (query["IdUser"] == null)
                    return;
                var id = (int)query["IdUser"];
                var todoItem = Todolist.Where(x => x.Id == id).FirstOrDefault();
            // Todolist.Remove(todoItem);
              if(todoItem.Val == 0)
                deleteOnDbCommand.Execute(todoItem);
                query = new Dictionary<string, object>();
           
        }
         if (Todo.Id == -1 && query.ContainsKey("NameUser"))
        {
            Console.WriteLine(Todo.Name);

            var element = (ToDoModel)query["NameUser"];

            if (element == null)
                return;
            ToSaveOnDB = element;
            Todolist.Add(element);
            query = new Dictionary<string, object>();
        }
        GetInitalData();
    }
    
}






