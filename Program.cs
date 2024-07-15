
/*
 StudentGrading
   │
   ├── Dependencies
   │
   ├── Subjects
   │   └── {a specific subject}.txt
   │
   └── Program.cs
 */


namespace StudentGrading
{

    public class Student
    {
        public string Name { get;}
        public List<string> Subjects = null!;

        public Student(string name)
        {
            Name = name;
        }

        public void Initialize_Subjects()
        {
            Subjects = Update_List_Of_Subjects();
        }
        private static List<string> Update_List_Of_Subjects()
        { List<string> subjects = new List<string>();
            try
            {
                var fileNames = Directory.GetFiles(Program.BaseDirectory);

                subjects.AddRange(from fileName in fileNames 
                    select Path.GetFileName(fileName) into path 
                    let indexComma = path.IndexOf('.') 
                    select path[..indexComma]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return subjects;
            
        } 
    }
    
    public static class Program
    {
        private static readonly string HomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static readonly string? ProjectFolder = Directory.GetParent(Directory.GetCurrentDirectory())?.Name;
        public static readonly string BaseDirectory = Path.Combine(HomeDirectory, ProjectFolder ?? string.Empty, "StudentGrading", "Subjects");


        private static List<Student> Initialize() { 
             var students = new List<Student>();
           try
           {
               var filePath = Path.Combine(BaseDirectory, "English.txt");
               using var sr = new StreamReader(filePath);
               while (sr.ReadLine() is { } line)
               {
                   var firstColon = line.IndexOf(';');
                   var studentName = line[..firstColon];
                   var student = new Student(studentName);
                   student.Initialize_Subjects();
                   students.Add(student);
               }
           }
           catch (Exception e) {
               Console.WriteLine("Exception: " + e.Message);
           } 
           
           return students;
         }

         private static void Display_Whole_Grades()
       {
           List<Student> students = Initialize();
            foreach (var student in students)
            {
                Console.WriteLine(student.Name);
                foreach (var subject in student.Subjects)
                {
                    var filePath = Path.Combine(BaseDirectory, $"{subject}.txt");
                    using var sr = new StreamReader(filePath);
                    while (sr.ReadLine() is { } line)
                    {
                        var firstColon = line.IndexOf(';');
                        if (student.Name == line[..firstColon])
                        {
                            firstColon++;
                            Console.WriteLine($"    {subject}:" + line[firstColon..].Replace(";", " "));
                        }
                    }
                }
            }
        }

         private static void Display_Specific_Subject()
       {
           var students = Initialize(); 
           for (var i = 0; i < students[0].Subjects.Count(); i++)
           {
               Console.WriteLine($"{i+1}. {students[0].Subjects[i]}");
           }
           Console.WriteLine("Enter a subject");
           var target = Console.ReadLine();

           if (int.TryParse(target, out _) && int.Parse(target) - 1 < students[0].Subjects.Count)
           {
               var filePath = Path.Combine(BaseDirectory, $"{students[0].Subjects[int.Parse(target)-1]}.txt");
               using var sr = new StreamReader(filePath);
               Console.WriteLine(students[0].Subjects[int.Parse(target)-1]);
               while (sr.ReadLine() is { } line)
               {
                   Console.WriteLine(line);
               }
           }
           else
           {
               Console.WriteLine("Invalid Input");
           }
       }

       private static void Find_Student()
        {
            var students = Initialize();
            var studentFullName = "";

            for(var i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i+1}. {students[i].Name}");
            }

            var input = Console.ReadLine();
            if (input != null)
            {
                var selectedIndex = int.Parse(input) -1;
                if (int.TryParse(input, out _) && selectedIndex < students.Count)
                {
                    studentFullName = students[selectedIndex].Name;

                }
                else
                {
                    Console.WriteLine("Invalid Input, try again.");
                }
            }


            Console.WriteLine(studentFullName);
            foreach (var subject in students[0].Subjects)
            {
                var filePath = Path.Combine(BaseDirectory, $"{subject}.txt");
                using var sr = new StreamReader(filePath);
                while (sr.ReadLine() is { } line)
                {
                    var firstColon = line.IndexOf(';');
                    if (studentFullName == line[..firstColon])
                    {
                        firstColon++;
                        Console.WriteLine($"   {subject}:" + line[firstColon..]);
                    }
                }
                sr.Close();
            }
            
        }

       private static (Dictionary<string, float>, Dictionary<float, string>) Get_Both_Extremes(bool isGettingBothExtremes = true)
        {
            Dictionary<string, float> studentGradeMap = new Dictionary<string, float>();
            Dictionary<float, string> reversedStudentGradeMap = new Dictionary<float, string>();

            var students = Initialize();
            
            foreach (var student in students)
            {
                studentGradeMap[student.Name] = 0;
                reversedStudentGradeMap[0] = student.Name;
            }

            foreach (var student in students)
            {
                float averageGrade = 0;
                foreach (var subject in student.Subjects)
                { var denominator  = 0; 
                    float tmp = 0;
                    var filePath = Path.Combine(BaseDirectory, $"{subject}.txt");;
                    using (var sr = new StreamReader(filePath))
                    {
                        while (sr.ReadLine() is { } line)
                        {
                            
                            var firstColon = line.IndexOf(";", StringComparison.Ordinal);
                            if (student.Name == line[..firstColon])
                            {
                                firstColon++;
                                for (var i = 0; i < line[firstColon..].Length; i++)
                                {
                                    var element = line[firstColon..][i].ToString();
                                    if (int.TryParse(element, out _ ))
                                    {
                                        tmp += int.Parse(element);
                                        denominator++;
                                    }
                                } 
                            }
                        }
                    }
                    averageGrade += (tmp / denominator);
                }
                averageGrade = (averageGrade / (students[0].Subjects.Count));
                studentGradeMap[student.Name] = averageGrade;
                reversedStudentGradeMap[averageGrade] = student.Name;
            }

            if (isGettingBothExtremes)
            {
                var maxAverage = studentGradeMap.Values.Max();
                Console.WriteLine("The Worst Student is " + reversedStudentGradeMap[maxAverage]);
                Console.WriteLine("The average is " + maxAverage.ToString("F2").Replace(",", "."));

                var minAverage = studentGradeMap.Values.Min();
                Console.WriteLine("The Best Student is " + reversedStudentGradeMap[minAverage]);
                Console.WriteLine("The average is " + minAverage.ToString("F2").Replace(",", "."));
            }

            return (studentGradeMap, reversedStudentGradeMap);
        }


       private static void Find_student_above_specificGrade(string userGrade)
        {
            try
            {
                var grade = float.Parse(userGrade); 
                Console.WriteLine(grade);
                var (gradeMap, reversedGradeMap) = Get_Both_Extremes(false);
                Console.WriteLine("Printing...");

                foreach (var entry in gradeMap.Where(entry => entry.Value > grade))
                {
                    Console.WriteLine();
                    Console.WriteLine(entry.Key + " : " + entry.Value.ToString().Replace(",", "."));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Input");
            }

        }

       private static void Calculate_Average_all_tests()
        {
            
            float result = 0;
            var students = Initialize();
            foreach (var subject in students[0].Subjects)
            {           
                float average = 0;
                var filePath = Path.Combine(BaseDirectory, $"{subject}.txt");;
                using var sr = new StreamReader(filePath);
                while (sr.ReadLine() is { } line)
                {
                    float tmp = 0;
                    var denominator = 0;
                    var firstColon = line.IndexOf(';');
                    firstColon++;
                    for (var i = 0; i < line[firstColon..].Length; i++)
                    {
                        var element = line[firstColon..][i].ToString();
                        if (int.TryParse(element, out _ ))
                        {
                            tmp += int.Parse(element);
                            denominator++;
                        }
                    }

                    average += tmp / denominator;
                }
                result += (float)average / students.Count;
            }
            Console.WriteLine("Average All Tests: " + ((result / students[0].Subjects.Count).ToString("F2").Replace(",", ".")));
        }
        
        
        // average: 2.17
        // 1.67, 3.00, 1.33, 2.67, 2.33, 2.00

        private static void Calculate_Average_a_test()
        {
            var students = Initialize();
            for (var i = 0; i < students[0].Subjects.Count(); i++)
            {
                Console.WriteLine($"{i+1}. {students[0].Subjects[i]}");
            }
            Console.WriteLine("Enter a subject");
            var target = Console.ReadLine();
            
            if (int.TryParse(target, out _) && int.Parse(target)-1 < students[0].Subjects.Count)
            {
                var subject = students[0].Subjects[int.Parse(target)-1];
                var filePath = Path.Combine(BaseDirectory, $"{subject}.txt");
                float average = 0;
                using (var sr = new StreamReader(filePath))
                {
                    while (sr.ReadLine() is { } line)
                    {
                        var denominator = 0;
                        float tmp = 0;
                        var firstColon = line.IndexOf(';');
                        firstColon++;
                        for (var i = 0; i < line[firstColon..].Length; i++)
                        {
                            var element = line[firstColon..][i].ToString();
                            if (int.TryParse(element, out _ ))
                            {
                                tmp += int.Parse(element);
                                denominator++;
                            }
                            
                        }

                        average += tmp / denominator;
                    }
                }
                average = average / students.Count;
                Console.WriteLine($"Average of {subject}: " + average.ToString("F2").Replace(",", "."));
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
            
        }

        private static void Add_new_test()
        {
            
            var students = Initialize();

            for (var i = 0; i < students[0].Subjects.Count(); i++)
            {
                Console.WriteLine($"{i+1}. {students[0].Subjects[i]}");
            }
            
            Console.WriteLine("Enter a subject");
            var target = Console.ReadLine();
            
            
            if (int.TryParse(target, out _) && int.Parse(target) < students[0].Subjects.Count)
            {
                var subject = students[0].Subjects[int.Parse(target)-1];
                var filePath = Path.Combine(BaseDirectory, $"{subject}.txt");;
                using var sr = new StreamReader(filePath);
                var lines = File.ReadAllLines(filePath);
                using var wr = new StreamWriter(filePath);
                foreach (var line in lines)
                {
                    var positionColon = line.IndexOf(";", StringComparison.Ordinal);
                    Console.WriteLine(line[..positionColon] + ": ");
                    var grade = Console.ReadLine();
                    wr.WriteLine(line + "; " + grade);
                }
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }

        private static void Sort_Students()
        {
            var optionNum = Console.ReadLine();
            if (int.TryParse(optionNum, out _) && int.Parse(optionNum) <= 2)
            {
                var (gradeMap, _) = Get_Both_Extremes(false);
                if (optionNum == "1")
                {
                    var sortedMap = gradeMap.OrderBy(x => x.Value);
                    foreach (var pair in sortedMap)
                    {
                        Console.WriteLine(pair.Key + ": " + pair.Value.ToString().Replace(",", "."));
                    }
                }
                else if (optionNum == "2")
                {
                    var sortedMap = gradeMap.OrderByDescending(x => x.Value);
                    foreach (var pair in sortedMap)
                    {
                        Console.WriteLine(pair.Key + ": " + pair.Value.ToString().Replace(",", "."));
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid Input");
            }
        }

        private static void Add_new_subject()
        {
            List<Student> students = Initialize();
            string directoryPath = $"{HomeDirectory}/{ProjectFolder}/StudentGrading/Subjects";
            Console.WriteLine("Enter a name of subject...");
            var newSubject = Console.ReadLine();
            if (students[0].Subjects.Contains(newSubject))
            {
                Console.WriteLine("It already exists.");
            }
            else
            {
                var fileName = $"{newSubject}.txt";
                var lines = new List<string>();
                foreach (var student in students)
                {
                    Console.WriteLine("Enter " + student.Name + "'s grade: ");
                    var grade = Console.ReadLine();
                    lines.Add(student.Name + "; " + grade);
                }

                File.WriteAllLines(Path.Combine(directoryPath, fileName), lines);
            }
        }


        private static void Main()
        {
            
            int select;
            do
            {
                Console.WriteLine("Enter to start...");
                Console.ReadLine();
              Console.WriteLine("Select Option: ");
              Console.WriteLine("1.Display whole grades");
              Console.WriteLine("2.Select Student to see his/her grade");
              Console.WriteLine("3.Find the best and worst grade based on Average");
              Console.WriteLine("4.Find people who have scored above a certain threshold");
              Console.WriteLine("5.Calculate average for the whole class across all test.");
              Console.WriteLine("6.Calculate average for the whole class for a given test");
              Console.WriteLine("7.Add new tests/grade");
              Console.WriteLine("8.Sort students based on their grades");
              Console.WriteLine("9.load a file that what you want ");
              Console.WriteLine("10. Add a new class");
              Console.WriteLine("11. Exit");
              select = int.Parse(Console.ReadLine());
              switch (select)
              {
                  case 1:
                      Display_Whole_Grades();
                      break;
                  case 2:
                      Console.WriteLine("Find grades for a specific student");
                      Console.WriteLine("Select a number:");
                      Find_Student();
                      break;
                  case 3:
                      Get_Both_Extremes();
                      break;
                  case 4:
                      Console.WriteLine("Enter a specific grade");
                      Find_student_above_specificGrade(Console.ReadLine());
                      break;
                  case 5 :
                      Calculate_Average_all_tests();
                      break;
                  case 6 :
                      Console.WriteLine("Enter a name of subject");
                      Calculate_Average_a_test();
                      break;
                  case 7:
                      Console.WriteLine("Enter a subject... ");
                      Add_new_test();
                      break;
                  case 8:
                      Console.WriteLine("Which options");
                      Console.WriteLine("1. Sort in ASC ");
                      Console.WriteLine("2. Sort in DES");
                      Sort_Students();
                      break;
                  case 9:
                      Console.WriteLine("Enter a subject");
                      Display_Specific_Subject();
                      break;
                  case 10: 
                      Add_new_subject();
                      break;
                  case 11:
                      Console.WriteLine("See ya!");
                      break;
                  default:
                      Console.WriteLine("Invalid select. You must select a valid option ranging from 1 to 9");
                      break;
              }
            } while(select != 11);
        }
        
    }
};