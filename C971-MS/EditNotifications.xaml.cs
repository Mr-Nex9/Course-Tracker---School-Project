namespace C971_MS;


//for the notification int stored in the database
//0 = off
//1 = start date on
//2 = end date on
//3= both notifications are on
public partial class EditNotifications : ContentPage
{
    DatabaseLogic db = new();
    string returnpage;
    int returnID;
    int newNotificationInt;
    bool pageLoaded = false;
    Course currentCourse;

    public EditNotifications(string type, int ID, Course course)
    {
        InitializeComponent();

        returnpage = type;
        returnID = ID;
        currentCourse = course;

        GetNotificationStatus();
    }

    async void GetNotificationStatus()
    {
        switch(returnpage)
        {
            case "Course":
                {
                    Label.Text = "Edit Notifications for " + currentCourse.CourseName;
                    switch (currentCourse.NotificationEnabled)
                    {
                        case 1:
                            {
                                Switch1.IsToggled = true;
                                Switch2.IsToggled = true;
                            }
                            break;
                        case 2:
                            {
                                Switch1.IsToggled = true;
                                Switch3.IsToggled = true;
                            }
                            break;
                        case 3:
                            {
                                Switch1.IsToggled = true;
                                Switch2.IsToggled = true;
                                Switch3.IsToggled = true;
                            }
                            break;
                    }
                }
                    break;
            case "New Course":
                {
                    Label.Text = "Edit Notifications for " + currentCourse.CourseName;
                    Switch1.IsToggled = false;
                    Switch2.IsToggled = false;
                    Switch3.IsToggled = false;
                }
                break;
            case "Assessment":
                {
                    var currentItem = await db.LookupAssessmentByID(returnID);
                    Label.Text = "Edit Notifications for " + currentItem.AssessmentName;
                    switch (currentItem.NotificationEnabled)
                    {
                        case 1:
                            {
                                Switch1.IsToggled = true;
                                Switch2.IsToggled = true;
                            }
                            break;
                        case 2:
                            {
                                Switch1.IsToggled = true;
                                Switch3.IsToggled = true;
                            }
                            break;
                        case 3:
                            {
                                Switch1.IsToggled = true;
                                Switch2.IsToggled = true;
                                Switch3.IsToggled = true;
                            }
                            break;
                    }
                }
                break;
        }


    }

    private async void SaveBtn_Clicked(object sender, EventArgs e)
    {
        switch (returnpage)
        {
            case "Course":
                {
                    currentCourse.NotificationEnabled = newNotificationInt;

                    db.UpdateCourse(currentCourse);

                    await Navigation.PushModalAsync(new EditCourse(true, -1, currentCourse));
                }
                break;
            case "New Course":
                {
                    currentCourse.NotificationEnabled = newNotificationInt;

                    await Navigation.PushModalAsync(new EditCourse(false, -1, currentCourse));
                }
                break;
            case "Assessment":
                {
                    Assessment Temp = await db.LookupAssessmentByID(returnID);
                    Temp.NotificationEnabled = newNotificationInt;

                    db.UpdateAssessment(Temp);
                    db.LoadNotifications();
                    Course CurCourse = await db.LookupCourseByID(Temp.CourseID);
                    await Navigation.PushModalAsync(new Assessments(CurCourse));
                }
                break;
        }
    }

    private async void CancelBtn_Clicked(object sender, EventArgs e)
    {
        switch (returnpage)
        {
            case "Course":
                {
                    await Navigation.PushModalAsync(new EditCourse(true, -1, currentCourse));
                }
                break;
            case "New Course":
                {
                    await Navigation.PushModalAsync(new EditCourse(false, -1, currentCourse));
                }
                break;
            case "Assessment":
                {
                    await Navigation.PushModalAsync(new Assessments(currentCourse));
                }
                break;
        }
    }

    private void OnToggled(object sender, ToggledEventArgs e)
    {
            if (Switch2.IsToggled)
            {
            Switch1.IsToggled = true;
                if (Switch3.IsToggled)
                {
                    newNotificationInt = 3;
                }
                else
                {
                    newNotificationInt = 1;
                }
            }
            else if (Switch3.IsToggled)
            {
                Switch1.IsToggled = true;
                newNotificationInt = 2;
            }
    }

    private void Switch1_Toggled(object sender, ToggledEventArgs e)
    {
        Console.WriteLine(pageLoaded);
        if(Switch1.IsToggled && pageLoaded == true)
        {
            Switch2.IsEnabled = true;
            Switch3.IsEnabled = true;

            Switch2.IsToggled = true;
            Switch3.IsToggled = true;

            newNotificationInt = 3;
        }
        else if(Switch1.IsToggled == false  && pageLoaded == true)
        {
            Switch2.IsEnabled = false;
            Switch3.IsEnabled = false;

            Switch2.IsToggled = false;
            Switch3.IsToggled = false;

            newNotificationInt = 0;
        }
        pageLoaded = true;
    }
}