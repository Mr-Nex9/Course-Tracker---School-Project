using static System.Net.Mime.MediaTypeNames;

namespace C971_MS;

public partial class CourseDetails : ContentPage
{
    DatabaseLogic db = new();
    Course CurCourse;
    public CourseDetails(Course currentCourse)
    {
        InitializeComponent();

        Console.WriteLine(currentCourse.ID);
        CurCourse = currentCourse;
        List<string> courseInfo = new();
        this.BindingContext = CurCourse;

        courseInfo.Add("Status: " + CurCourse.Status);
        courseInfo.Add("Start Date: " + CurCourse.StartDate.Date.ToString("d"));
        courseInfo.Add("End Date: " + CurCourse.EndDate.Date.ToString("d"));
        courseInfo.Add("Instructor: " + CurCourse.InstructorName);
        courseInfo.Add("Instructor Email: " + CurCourse.InstructorEmail);
        courseInfo.Add("Instructor Phone: " + CurCourse.InstructorPhone);
        if (CurCourse.NotificationEnabled > 0)
        {
            courseInfo.Add("Notifications: ON");
        }
        else
        {
            courseInfo.Add("Notifications: OFF");
        }

        CourseInfo.ItemsSource = courseInfo;

    }

    private void EditBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new EditCourse(true, CurCourse.TermID,CurCourse));
    }

    private void AssessmentBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new Assessments(CurCourse));
    }

    private async void TermBtn_Clicked_1(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new MainPage(CurCourse.TermID));
    }

    private async void ShareBtn_Clicked(object sender, EventArgs e)
    {
        if (CurCourse.Notes.Length > 0)
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = CurCourse.Notes
            });
        }
    }

    private void NotesBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        db.UpdateCourse(CurCourse);
    }
}