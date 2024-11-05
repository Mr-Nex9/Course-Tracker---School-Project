using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plugin.LocalNotification;


namespace C971_MS
{


    #region
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int ID { get; set; }
        public string TermName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int ID { get; set; }
        public int TermID { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public string Notes { get; set; }
        public int NotificationEnabled { get; set; }
    }

    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int ID { get; set; }
        public int CourseID { get; set; }
        public string AssessmentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }

        public int NotificationEnabled { get; set; }
    }
    #endregion
    public class DatabaseLogic
    {
        private SQLiteAsyncConnection conn;
        static int CurrentTermID = 1;
        public int GetCurrentTerm() { return CurrentTermID; }
        static bool Loaded = false;
        //Database creation and Location
        #region
        const string fileName = "SQLiteDB.db3";
        public const SQLite.SQLiteOpenFlags Flags =
           // open the database in read/write mode
           SQLite.SQLiteOpenFlags.ReadWrite |
           // create the database if it doesn't exist
           SQLite.SQLiteOpenFlags.Create |
           // enable multi-threaded database access
           SQLite.SQLiteOpenFlags.SharedCache;
        public static string path =>
            Path.Combine(FileSystem.AppDataDirectory, fileName);
        #endregion

        //Load database 
        public async Task<bool> LoadDatabase()
        {
            if (Loaded)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Database Created!");
                conn = new SQLiteAsyncConnection(path, Flags);

                await conn.CreateTableAsync<Term>();
                await conn.CreateTableAsync<Course>();
                await conn.CreateTableAsync<Assessment>();

                List<Term> termlist = await conn.Table<Term>().ToListAsync();



                //Insert testing items if database is empty
                if (termlist.Count == 0)
                {
                    Console.WriteLine("DB Empty. Populating Test Data");

                    Term testTerm = new();
                    testTerm.TermName = "Term 1";
                    testTerm.StartDate = new DateTime(2024, 07, 01);
                    testTerm.EndDate = new DateTime(2024, 12, 31);
                    AddTerm(testTerm);

                    Course testCourse = new();
                    testCourse.TermID = await GetTermID(testTerm.TermName);
                    testCourse.CourseName = "Mobile Application Development Using C# - C971";
                    testCourse.Status = "In-Progress";
                    testCourse.StartDate = new DateTime(2024, 07, 01);
                    testCourse.EndDate = new DateTime(2024, 07, 31);
                    testCourse.InstructorName = "Anika Patel";
                    testCourse.InstructorPhone = "555-123-4567";
                    testCourse.InstructorEmail = "anika.patel@strimeuniversity.edu";
                    AddCourse(testCourse);
                    testCourse = await GetCourseInfo(testCourse.CourseName);

                    Assessment testPA = new();
                    testPA.CourseID = testCourse.ID;
                    testPA.AssessmentName = "Test PA";
                    testPA.StartDate = new DateTime(2024, 07, 01);
                    testPA.EndDate = new DateTime(2024, 12, 31);
                    testPA.Type = "Performance";
                    AddAssessment(testPA);

                    Assessment testOA = new();
                    testOA.CourseID = testCourse.ID;
                    testOA.AssessmentName = "Test OA";
                    testOA.StartDate = new DateTime(2024, 07, 01);
                    testOA.EndDate = new DateTime(2024, 12, 31);
                    testOA.Type = "Objective";
                    AddAssessment(testOA);
                }

                termlist = await conn.Table<Term>().ToListAsync();
                List<Course> courselist = await conn.Table<Course>().ToListAsync();
                List<Assessment> assessmentlist = await conn.Table<Assessment>().ToListAsync();

                Console.WriteLine("All terms loaded");


                Console.WriteLine(termlist.Count);
                Console.WriteLine(courselist.Count);
                Console.WriteLine(assessmentlist.Count);

                foreach (Assessment item in assessmentlist)
                { Console.WriteLine(item.CourseID); }

                if (await EnableNotifications())
                {
                    LoadNotifications();
                }

                Loaded = true;
                return true;


            }
        }

        public async Task<bool> EnableNotifications()
        {
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                if(await LocalNotificationCenter.Current.RequestNotificationPermission())
                {
                    return true; 
                }

            }
            else
            {
                return true;
            }
            return false;
        }
        public async void LoadNotifications()
        {
            //notification ID logic 
            //first number is either course (1) or assessment(2)
            //then the ID of the item
            //then a number for start(1) or end(2)
            TimeSpan Hour = DateTime.Now.TimeOfDay;
            Hour += TimeSpan.FromMinutes(1);

            LocalNotificationCenter.Current.CancelAll();

            List<Course> courseList = await conn.Table<Course>().Where(v => v.NotificationEnabled > 0).ToListAsync();
            foreach (Course course in courseList)
            {
                switch (course.NotificationEnabled)
                {
                    case 1:
                        {
                            string temp = "1" + course.ID + "1";

                            var request = new NotificationRequest
                            {
                                NotificationId = Convert.ToInt32(temp),
                                Title = "Your course starts today!",
                                Description = $"{course.CourseName} Start",
                                Schedule = new NotificationRequestSchedule
                                {
                                    NotifyTime = course.StartDate + Hour,
                                }
                            };
                            await LocalNotificationCenter.Current.Show(request);
                        }
                        break;
                    case 2:
                        {
                            string temp = "1" + course.ID + "2";

                            var request = new NotificationRequest
                            {
                                NotificationId = Convert.ToInt32(temp),
                                Title = "Your Course ends today!",
                                Description = $"{course.CourseName} End",
                                Schedule = new NotificationRequestSchedule
                                {
                                    NotifyTime = course.EndDate + Hour,
                                }
                            };
                            await LocalNotificationCenter.Current.Show(request);
                        }
                        break;
                    case 3:
                        {
                            string temp = "1" + course.ID + "1";

                            var request = new NotificationRequest
                            {
                                NotificationId = Convert.ToInt32(temp),
                                Title = "Your course starts today!",
                                Description = $"{course.CourseName} Start",
                                Schedule = new NotificationRequestSchedule
                                {
                                    NotifyTime = course.StartDate + Hour,
                                }
                            };
                            await LocalNotificationCenter.Current.Show(request);

                            temp = "1" + course.ID + "2";

                            request = new NotificationRequest
                            {
                                NotificationId = Convert.ToInt32(temp),
                                Title = "Your Course ends today!",
                                Description = $"{course.CourseName} End",
                                Schedule = new NotificationRequestSchedule
                                {
                                    NotifyTime = course.EndDate + Hour,
                                }
                            };
                            await LocalNotificationCenter.Current.Show(request);
                        }
                        break;
                }
            }

            List<Assessment> assessmentList = await conn.Table<Assessment>().Where(v => v.NotificationEnabled > 0).ToListAsync();
            foreach (Assessment assessment in assessmentList)
            {
                switch (assessment.NotificationEnabled)
                {
                    case 1:
                        {
                            string temp = "2" + assessment.ID + "1";

                            var request = new NotificationRequest
                            {
                                NotificationId = Convert.ToInt32(temp),
                                Title = "Your assessment starts today!",
                                Description = $"{assessment.AssessmentName} starts",
                                Schedule = new NotificationRequestSchedule
                                {
                                    NotifyTime = assessment.StartDate + Hour,
                                }
                            };
                            await LocalNotificationCenter.Current.Show(request);
                        }
                        break;
                    case 2:
                        {
                            string temp = "2" + assessment.ID + "2";

                            var request = new NotificationRequest
                            {
                                NotificationId = Convert.ToInt32(temp),
                                Title = "Your assessment ends today!",
                                Description = $"{assessment.AssessmentName} ends",
                                Schedule = new NotificationRequestSchedule
                                {
                                    NotifyTime = assessment.EndDate + Hour,
                                }
                            };
                            await LocalNotificationCenter.Current.Show(request);
                        }
                        break;
                    case 3:
                        {
                            string temp = "2" + assessment.ID + "1";

                            var request = new NotificationRequest
                            {
                                NotificationId = Convert.ToInt32(temp),
                                Title = "Your assessment starts today!",
                                Description = $"{assessment.AssessmentName} starts",
                                Schedule = new NotificationRequestSchedule
                                {
                                    NotifyTime = assessment.StartDate + Hour,
                                }
                            };
                            await LocalNotificationCenter.Current.Show(request);

                            temp = "2" + assessment.ID + "2";

                            request = new NotificationRequest
                            {
                                NotificationId = Convert.ToInt32(temp),
                                Title = "Your assessment ends today!",
                                Description = $"{assessment.AssessmentName} ends",
                                Schedule = new NotificationRequestSchedule
                                {
                                    NotifyTime = assessment.EndDate + Hour,
                                }
                            };
                            await LocalNotificationCenter.Current.Show(request);
                        }
                        break;
                }
            }
        }
        //Retrieval Methods
        #region
        public async Task<Course> GetCourseInfo(string courseName)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            return await conn.Table<Course>().Where(v => v.CourseName == courseName).FirstOrDefaultAsync();

        }

        public async Task<List<Course>> GetTermInfo(int termID)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            Console.WriteLine(termID);
            List<Course> courseList = new();

            List<Course> tempList = await conn.Table<Course>().ToListAsync();

            Console.WriteLine(tempList.Count);

            foreach (var item in tempList)
            {
                if (item.TermID == termID)
                {
                    courseList.Add(item);
                }
            }
            return courseList;
        }
        public async Task<int> GetTermID(string termname)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            Term currentTerm = await conn.Table<Term>().Where(v => v.TermName == termname).FirstOrDefaultAsync();

            return currentTerm.ID;
        }
        public async Task<List<Assessment>> GetAssessments(int courseID)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            List<Assessment> assessmentList = await conn.Table<Assessment>().Where(v => v.CourseID == courseID).ToListAsync();

            return assessmentList;
        }
        public async Task<Assessment> LookupAssessmentByName(string assessmentName)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            List<Assessment> assessmentList = await conn.Table<Assessment>().ToListAsync();

            foreach (Assessment item in assessmentList)
            {
                if (item.AssessmentName == assessmentName)
                    return item;
            }
            return null;
        }
        public async Task<Assessment> LookupAssessmentByID(int id)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            List<Assessment> assessmentList = await conn.Table<Assessment>().ToListAsync();

            foreach (Assessment item in assessmentList)
            {
                if (item.ID == id)
                    return item;
            }
            return null;
        }
        public async Task<Course> LookupCourseByID(int ID)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            return await conn.Table<Course>().Where(v => v.ID == ID).FirstOrDefaultAsync();
        }
        public async Task<List<Term>> GetTerms()
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            List<Term> termList = await conn.Table<Term>().ToListAsync();
            

            return termList;
        }
        public async Task<Term> LookupTermByName (string name)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            return await conn.Table<Term>().Where(v => v.TermName == name).FirstOrDefaultAsync();
            
        }

        public async Task<Term> LookupTermByID(int ID)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            return await conn.Table<Term>().Where(v => v.ID == ID).FirstOrDefaultAsync();
        }
        #endregion

        //Addition Methods
        #region
        public async void AddCourse(Course course)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            await conn.InsertAsync(course);
        }
        public async void AddTerm(Term term)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            await conn.InsertAsync(term);
        }
        public async void AddAssessment(Assessment assessment)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            await conn.InsertAsync(assessment);
        }
        #endregion

        //Deletion Methods
        #region
        public async Task<bool> DeleteCourse(Course course)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            List<Assessment> assessmentList = await conn.Table<Assessment>().Where(v => v.CourseID == course.ID).ToListAsync();

            foreach (var item in assessmentList)
            {
                await conn.DeleteAsync(item);
            }

            await conn.DeleteAsync(course);
            return true;
        }
        public async Task<bool> DeleteTerm(Term term)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            List<Course> courseList = await conn.Table<Course>().Where(v => v.TermID == term.ID).ToListAsync();

            foreach (var item in courseList)
            {
                DeleteCourse(item);
            }

            await conn.DeleteAsync(term);
            return true;
        }
        public async Task<bool> DeleteAssessment(Assessment assessment)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            await conn.DeleteAsync(assessment);
            return true;
        }
        #endregion

        //Update Methods
        #region
        public async void UpdateAssessment(Assessment CurAssessment)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            await conn.UpdateAsync(CurAssessment);
        }
        public async void UpdateCourse(Course CurCourse)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            await conn.UpdateAsync(CurCourse);
        }
        public async void UpdateTerm(Term CurTerm)
        {
            conn = new SQLiteAsyncConnection(path, Flags);
            await conn.UpdateAsync(CurTerm);
        }
        #endregion
    }
}
