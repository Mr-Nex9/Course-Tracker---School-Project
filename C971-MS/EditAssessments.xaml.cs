using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;

namespace C971_MS;

public partial class EditAssessments : ContentPage
{
    //hasPAorOA info
    //0 = neither
    //1 = has PA
    //2 = has OA
    public Assessment curAssessment;
    DatabaseLogic db = new();
    bool ismod;
    List<string> types = new List<string> {"Performance", "Objective" };
    public EditAssessments(bool isModification, Assessment assessment,int hasPAorOA)
	{
		InitializeComponent();
        curAssessment = assessment;
        switch (hasPAorOA)
        {
            case 1:
            {
                    types.Remove("Performance");
            }break;
            case 2:
            {
                    types.Remove("Objective");
                }
                break;
        }

        TypePicker.ItemsSource = types;

        ismod = isModification;
        if (ismod)
        {
            Label.Text = "Edit Assessment";
        }
        else
        {
            Label.Text = "Add Assessment";
        }
        this.BindingContext = curAssessment;
	}
    private async void SetupFields()
    {
        Course course = await db.LookupCourseByID(curAssessment.CourseID);

        StartDatePicker.MinimumDate = course.StartDate;
        StartDatePicker.MaximumDate = course.EndDate;
        EndDatePicker.MaximumDate = course.EndDate;
        EndDatePicker.MinimumDate = course.StartDate.AddDays(1);
    }
    private async void DeleteBtn_Clicked(object sender, EventArgs e)
    {
        Course CurCourse = await db.LookupCourseByID(curAssessment.CourseID);
        await db.DeleteAssessment(curAssessment);
        await Navigation.PushModalAsync(new Assessments(CurCourse));
    }

    private async void SaveBtn_Clicked(object sender, EventArgs e)
    {
        if (NameEditor.Text.IsNullOrEmpty() || TypePicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Please fill in all fields!", "OK");
        }
        else if(StartDatePicker.Date >= EndDatePicker.Date)
        {
            await DisplayAlert("Error", "End date must be after start date!", "OK");
        }
        else
        {
            curAssessment.AssessmentName = NameEditor.Text;
            curAssessment.StartDate = StartDatePicker.Date;
            curAssessment.EndDate = EndDatePicker.Date;
            curAssessment.Type = TypePicker.SelectedItem.ToString();

            Course CurCourse = await db.LookupCourseByID(curAssessment.CourseID);

            if (ismod)
            {
                db.UpdateAssessment(curAssessment);
            }
            else
            {
                db.AddAssessment(curAssessment);
            }

            db.LoadNotifications();
            await Navigation.PushModalAsync(new Assessments(CurCourse));
        }
    }

    private async void CancelBtn_Clicked(object sender, EventArgs e)
    {
        Course CurCourse = await db.LookupCourseByID(curAssessment.CourseID);
        await Navigation.PushModalAsync(new Assessments(CurCourse));
    }
}