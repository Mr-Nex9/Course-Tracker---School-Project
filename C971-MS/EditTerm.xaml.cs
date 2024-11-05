namespace C971_MS;

public partial class EditTerm : ContentPage
{
    DatabaseLogic db = new DatabaseLogic();
    int CurrentTerm;
    Course SelectedCourse;
    List<Course> courseList = new();
    Button button;

    public EditTerm(int termID)
    {
        InitializeComponent();
        CurrentTerm = termID;
        GetCourseList();

    }

    async void GetCourseList()
    {
        await db.LoadDatabase();

        CourseContainer.Children.Clear();

        courseList = await db.GetTermInfo(CurrentTerm);

        foreach (Course item in courseList)
        {
            var btn = new Button { Text = $"{item.CourseName}: {item.StartDate.Date} -{item.EndDate.Date} " };
            btn.StyleId = item.CourseName;
            btn.Clicked += OnButtonClicked;
            btn.Background = Colors.LightGray;
            btn.BorderWidth = 1;
            btn.BorderColor = Colors.Black;
            btn.TextColor = Colors.Black;
            CourseContainer.Children.Add(btn);
        }
    }
    private async void OnButtonClicked(object sender, EventArgs e)
    {
        if (button != null)
            button.Background = Colors.LightGray;

        button = (Button)sender;
        button.Background = Colors.Blue;
        SelectedCourse = await db.GetCourseInfo(button.StyleId);
    }

    private void SaveBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new MainPage(db.GetCurrentTerm()));
    }

    private async void DeleteBtn_Clicked(object sender, EventArgs e)
    {
        if (SelectedCourse != null)
        {
            await db.DeleteCourse(SelectedCourse);
            await Navigation.PushModalAsync(new EditTerm(CurrentTerm)); //refresh page
        }
        else
        {
            await DisplayAlert("Error", "No Course currently selected!", "OK");
        }
    }
}