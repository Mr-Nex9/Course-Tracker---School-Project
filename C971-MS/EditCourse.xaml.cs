using Microsoft.IdentityModel.Tokens;

namespace C971_MS;

public partial class EditCourse : ContentPage
{
    DatabaseLogic db = new();
    int returnPage;
    Course currentCourse;
    static int SavedReturnPage;
    List<string> StatusTypes = new List<string> { "In-Progress", "Planned","Completed", "Dropped"};
    bool IsMod;
    public EditCourse(bool isMod, int pageID, Course course)
    {
        InitializeComponent();
        currentCourse = course;
        IsMod = isMod;
        if (pageID > 0)
        {
            returnPage = pageID;
            currentCourse.TermID = pageID;
        }
        else
        {
            returnPage = SavedReturnPage;
        }

        StatusPicker.ItemsSource = StatusTypes;

        this.BindingContext = currentCourse;


        if (currentCourse.NotificationEnabled >0)
        {
            NotificationsBtn.Text = "ON";
        }
        else
        {
            NotificationsBtn.Text = "OFF";
        }
    }

    private async void SaveBtn_Clicked(object sender, EventArgs e)
    {
        if (NameEntry.Text.IsNullOrEmpty() || Instructor.Text.IsNullOrEmpty() || Email.Text.IsNullOrEmpty() || Phone.Text.IsNullOrEmpty())
        {
            await DisplayAlert("Error", "Please fill in all boxes!", "OK");
        }
        else if (currentCourse.StartDate >= currentCourse.EndDate)
        {
            await DisplayAlert("Error", "End date must be after start date!", "OK");
        }
        else
        { 
            if (IsMod)
            {
                Console.WriteLine("Modifying!");
                db.UpdateCourse(currentCourse);
                db.LoadNotifications();
               await  Navigation.PushModalAsync(new CourseDetails(currentCourse));
            }
            else
            {
                Console.WriteLine("Adding!");
                db.AddCourse(currentCourse);
                db.LoadNotifications();
                await Navigation.PushModalAsync(new CourseDetails(currentCourse));
            }
        }
    }

    private void CancelBtn_Clicked(object sender, EventArgs e)
    {
        if (returnPage == -1 || IsMod)
        {
            Navigation.PushModalAsync(new CourseDetails(currentCourse));
        }
        else
        {
            Navigation.PushModalAsync(new MainPage(returnPage));
        }
    }

    private void Notifications_Clicked(object sender, EventArgs e)
    {
        SavedReturnPage = returnPage;
        if (IsMod)
        {
            Navigation.PushModalAsync(new EditNotifications("Course", currentCourse.ID, currentCourse));
        }
        else
        {
            Navigation.PushModalAsync(new EditNotifications("New Course", currentCourse.ID, currentCourse));
        }
    }
}