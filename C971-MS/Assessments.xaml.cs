namespace C971_MS;

public partial class Assessments : ContentPage
{
    Course currentCourse;
    DatabaseLogic db = new();
    bool hasPA = false;
    bool hasOA = false;
    bool isShowingInfo1 = false;
    bool isShowingInfo2 = false;
    Assessment curAssessment;
    Button button;
    public Assessments(Course course)
    {
        InitializeComponent();
        currentCourse = course;
        
        Label.Text = currentCourse.CourseName;

        GetAssessmentList();
    }
    async void GetAssessmentList()
    {
        List<Assessment> AssessmentList = await db.GetAssessments(currentCourse.ID);
        Console.WriteLine(AssessmentList.Count);
        int count = 0;

        foreach (Assessment item in AssessmentList)
        {
            Console.WriteLine(item.AssessmentName);


            var btn = new Button { Text = $"{item.AssessmentName}" };
            btn.Clicked += OnButtonClicked;
            btn.Background = Colors.LightGray;
            btn.BorderWidth = 1;
            btn.BorderColor = Colors.Black;
            btn.TextColor = Colors.Black;

            if (count == 0)//first assessment
            {
                btn.StyleId = "1"; 
                Assessment1.Children.Add(btn);
                count++;

            }
            else //second assessment
            {
                btn.StyleId = "2";
                Assessment2.Children.Add(btn);

            }

            if (item.Type == "Performance")
            {
                hasPA = true;
            }
            else
            {
                hasOA = true;
            }
        }
    }
    private async void OnButtonClicked(object? sender, EventArgs e)
    {
        if (button != null)
            button.Background = Colors.LightGray;

        button = sender as Button;
        button.Background = Colors.Blue;

        if (isShowingInfo1 == false && button.StyleId == "1")
        {
 
            curAssessment = await db.LookupAssessmentByName(button.Text);
            List<string> info = new();

            info.Add(curAssessment.Type);
            info.Add("Start Date: " + curAssessment.StartDate.Date.ToString("yyyy-MM-dd"));
            info.Add("End Date: " + curAssessment.EndDate.Date.ToString("yyyy-MM-dd"));

            var listview = new ListView();
            listview.ItemsSource = info;
            listview.StyleId = "InfoList";

            string notiInfo;

            if (curAssessment.NotificationEnabled > 0)
            {
                notiInfo = "Notifications: ON";
            }
            else
            {
                notiInfo = "Notifications: OFF";
            }


            var btn = new Button { Text = $"{notiInfo}" };
            btn.StyleId = curAssessment.AssessmentName;
            btn.Clicked += OnNotificationButtonClicked;
            btn.Background = Colors.LightGray;
            btn.BorderWidth = 1;
            btn.BorderColor = Colors.Black;
            btn.TextColor = Colors.Black;

            Assessment1.Children.Add(listview);
            Assessment1.Children.Add(btn);
            isShowingInfo1 = true;

        }
        else if (isShowingInfo2 == false && button.StyleId == "2")
        {
            curAssessment = await db.LookupAssessmentByName(button.Text);
            List<string> info = new();

            info.Add(curAssessment.Type + " Assessment");
            info.Add("Start Date: " + curAssessment.StartDate.Date.ToString("yyyy-MM-dd"));
            info.Add("End Date: " + curAssessment.EndDate.Date.ToString("yyyy-MM-dd"));

            var listview = new ListView();
            listview.ItemsSource = info;
            listview.StyleId = "InfoList";

            string notiInfo;

            if (curAssessment.NotificationEnabled > 0)
            {
                notiInfo = "Notifications: ON";
            }
            else
            {
                notiInfo = "Notifications: OFF";
            }

            var btn = new Button { Text = $"{notiInfo}" };
            btn.StyleId = curAssessment.AssessmentName;
            btn.Clicked += OnNotificationButtonClicked;
            btn.Background = Colors.LightGray;
            btn.BorderWidth = 1;
            btn.BorderColor = Colors.Black;
            btn.TextColor = Colors.Black;


            Assessment2.Children.Add(listview);
            Assessment2.Children.Add(btn);
            isShowingInfo2 = true;
        
        }

    }
    private async void OnNotificationButtonClicked(object? sender, EventArgs e)
    {
        var btn = sender as Button;
        Assessment temp = await db.LookupAssessmentByName(btn.StyleId);

        await Navigation.PushModalAsync(new EditNotifications("Assessment", temp.ID, currentCourse));
    }
    private void CourseBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new CourseDetails(currentCourse));
    }

    private async void EditBtn_Clicked(object sender, EventArgs e)
    {
        Console.WriteLine(curAssessment.AssessmentName);
        int temp = 0;
        if (hasOA && hasPA)
        {
            if(curAssessment.Type == "Performance")
            {
                temp = 2;
            }
            else
            {
                temp = 1;
            }
        }
        if (curAssessment != null)
        {
            await Navigation.PushModalAsync(new EditAssessments(true, curAssessment,temp));
        }
        else
        {
            await DisplayAlert("Error", "No Assessment currently selected!", "OK");

        }
    }

    private async void AddBtn_Clicked(object sender, EventArgs e)
    {
        if (hasOA &&hasPA)
        {
            await DisplayAlert("Error", "Assessment limit reached!", "OK");
        }
        else if (hasOA)
        {
            Assessment temp = new();
            temp.CourseID = currentCourse.ID;
            await Navigation.PushModalAsync(new EditAssessments(false, temp, 2));
        }
        else if (hasPA)
        {
            Assessment temp = new();
            temp.CourseID = currentCourse.ID;
            await Navigation.PushModalAsync(new EditAssessments(false, temp, 1));
        }
        else
        {
            Assessment temp = new();
            temp.CourseID = currentCourse.ID;
            await Navigation.PushModalAsync(new EditAssessments(false, temp, 0));
        }


    }
}