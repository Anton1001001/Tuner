using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Tunerv1._0
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private SoundCapturer _soundCapturer;
        private const int RequestRecordAudioPermissionCode = 100;
        private static double freq;
        private static object lockObject = new object(); 
        
        private int stringButtonsLayoutIndex;
        private ViewGroup mainView;

        private DrawingView drawingView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            RelativeLayout guitarButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.guitarButtonsLayout);
            View guitarButtonsView = guitarButtonsLayout;
            mainView = (ViewGroup)guitarButtonsView.Parent;
            stringButtonsLayoutIndex = mainView.IndexOfChild(guitarButtonsLayout);
            
            Spinner instrumentSpinner = FindViewById<Spinner> (Resource.Id.instrumentsSpinner);
            
            instrumentSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (instrumentSpinner_ItemSelected);
            var instrumentsAdapter = ArrayAdapter.CreateFromResource (this, Resource.Array.instruments_array, Android.Resource.Layout.SimpleSpinnerItem);

            instrumentsAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            instrumentSpinner.Adapter = instrumentsAdapter;
            
            
            FrameLayout drawingContainer = FindViewById<FrameLayout>(Resource.Id.drawingContainer);
            
            drawingView = new DrawingView(this);
            drawingContainer.AddView(drawingView);
            
            _soundCapturer = new SoundCapturer();
            _soundCapturer.FrequencyDetected += SoundCapturer_FrequencyDetected;
            
            if (CheckSelfPermission(Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                RequestPermissions(new [] { Manifest.Permission.RecordAudio }, RequestRecordAudioPermissionCode);
            }
            
            _soundCapturer.StartCapture();
   
        }
        

        
        private void SetupLayout(View layout, Button button, GuitarString guitarString)
        {
            button.Click += (sender, e) =>
            {
                drawingView.Frequency = guitarString.Frequency;
                drawingView.minFrequency = guitarString.Frequency - 20;
                drawingView.maxFrequency = guitarString.Frequency + 20;
                drawingView.Note = guitarString.Note;
            };
        }
        private void SetupInstrumentLayout(View instrumentLayout, GuitarString[] guitarStrings)
        {
            for (int i = 0; i < guitarStrings.Length; i++)
            {
                Button button = instrumentLayout.FindViewById<Button>(Resource.Id.button1 + i);
                SetupLayout(instrumentLayout, button, guitarStrings[i]);
                button.Text = guitarStrings[i].Note;
            }
        }
        

        private View guitarLayout;
        private View basLayout;
        private View celloLayout;
        private View violinLayout;
        private View ukuleleLayout;
        private View guitar7Layout;
        public static GuitarString[] tuningNotes;
        private void instrumentSpinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string instrument = (string)spinner.GetItemAtPosition(e.Position);

            switch (instrument)
            {
                case "Guitar":
                    guitarLayout = LayoutInflater.Inflate(Resource.Layout.guitar_buttons_layout, mainView, false);
                    RelativeLayout guitarButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.guitarButtonsLayout);
                    
                    mainView.RemoveViewAt(stringButtonsLayoutIndex);

                    mainView.AddView(guitarLayout, stringButtonsLayoutIndex);
            
                    Spinner tuningGuitarSpinner = FindViewById<Spinner>(Resource.Id.tuningSpinner);
                    tuningGuitarSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(tuningGuitarSpinner_ItemSelected);
                    var instrumentsGuitarAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.guitar_tuning_array, Android.Resource.Layout.SimpleSpinnerItem);
                    instrumentsGuitarAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    tuningGuitarSpinner.Adapter = instrumentsGuitarAdapter;
                    break;
                
                case "Violin":
                    violinLayout = LayoutInflater.Inflate(Resource.Layout.violin_buttons_layout, mainView, false);
                    RelativeLayout violinButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.violinButtonsLayout);
                    
                    mainView.RemoveViewAt(stringButtonsLayoutIndex);

                    mainView.AddView(violinLayout, stringButtonsLayoutIndex);
                    
                    Spinner tuningViolinSpinner = FindViewById<Spinner> (Resource.Id.tuningSpinner);
            
                    tuningViolinSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (tuningViolinSpinner_ItemSelected);
                    var instrumentsViolinAdapter = ArrayAdapter.CreateFromResource (this, Resource.Array.violin_tuning_array, Android.Resource.Layout.SimpleSpinnerItem);

                    instrumentsViolinAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    tuningViolinSpinner.Adapter = instrumentsViolinAdapter;
                    
                    break;
                case "Bas-guitar":
                    basLayout = LayoutInflater.Inflate(Resource.Layout.bas_buttons_layout, mainView, false);
                    RelativeLayout basButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.basButtonsLayout);
                    
                    mainView.RemoveViewAt(stringButtonsLayoutIndex);

                    mainView.AddView(basLayout, stringButtonsLayoutIndex);
            
                    Spinner tuningBasSpinner = FindViewById<Spinner>(Resource.Id.tuningSpinner);
                    tuningBasSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(tuningBasSpinner_ItemSelected);
                    var instrumentsBasAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.bas_tuning_array, Android.Resource.Layout.SimpleSpinnerItem);
                    instrumentsBasAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    tuningBasSpinner.Adapter = instrumentsBasAdapter;
                    break;
                
                case "Cello":
                    celloLayout = LayoutInflater.Inflate(Resource.Layout.cello_buttons_layout, mainView, false);
                    RelativeLayout celloButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.celloButtonsLayout);
                    
                    mainView.RemoveViewAt(stringButtonsLayoutIndex);

                    mainView.AddView(celloLayout, stringButtonsLayoutIndex);
            
                    Spinner tuningCelloSpinner = FindViewById<Spinner>(Resource.Id.tuningSpinner);
                    tuningCelloSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(tuningCelloSpinner_ItemSelected);
                    var instrumentsCelloAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.cello_tuning_array, Android.Resource.Layout.SimpleSpinnerItem);
                    instrumentsCelloAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    tuningCelloSpinner.Adapter = instrumentsCelloAdapter;
                    break;
                case "Ukulele":
                    ukuleleLayout = LayoutInflater.Inflate(Resource.Layout.ukulele_buttons_layout, mainView, false);
                    RelativeLayout ukuleleButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.ukuleleButtonsLayout);
                    
                    mainView.RemoveViewAt(stringButtonsLayoutIndex);

                    mainView.AddView(ukuleleLayout, stringButtonsLayoutIndex);
            
                    Spinner tuningUkuleleSpinner = FindViewById<Spinner>(Resource.Id.tuningSpinner);
                    tuningUkuleleSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(tuningUkuleleSpinner_ItemSelected);
                    var instrumentsUkuleleAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.ukulele_tuning_array, Android.Resource.Layout.SimpleSpinnerItem);
                    instrumentsUkuleleAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    tuningUkuleleSpinner.Adapter = instrumentsUkuleleAdapter;
                    break;
                case "Guitar 7":
                    guitar7Layout = LayoutInflater.Inflate(Resource.Layout.guitar7_buttons_layout, mainView, false);
                    RelativeLayout guitar7ButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.guitar7ButtonsLayout);
                    
                    mainView.RemoveViewAt(stringButtonsLayoutIndex);

                    mainView.AddView(guitar7Layout, stringButtonsLayoutIndex);
            
                    Spinner tuningGuitar7Spinner = FindViewById<Spinner>(Resource.Id.tuningSpinner);
                    tuningGuitar7Spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(tuningGuitar7Spinner_ItemSelected);
                    var instrumentsGuitar7Adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.guitar7_tuning_array, Android.Resource.Layout.SimpleSpinnerItem);
                    instrumentsGuitar7Adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    tuningGuitar7Spinner.Adapter = instrumentsGuitar7Adapter;
                    break;
            }
        }
        
        private void tuningGuitar7Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string tuning = (string)spinner.GetItemAtPosition(e.Position);

            switch (tuning)
            {
                case "Standard":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(61.74f, "B"),
                        new GuitarString(82.41f, "E"),
                        new GuitarString(110.00f, "A"),
                        new GuitarString(147.83f, "D"),
                        new GuitarString(196.00f, "G"),
                        new GuitarString(246.96f, "B"),
                        new GuitarString(329.63f, "E")
                    };
                    break;
                case "Drop A":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(55.00f, "A"),
                        new GuitarString(82.41f, "E"),
                        new GuitarString(110.00f, "A"),
                        new GuitarString(147.83f, "D"),
                        new GuitarString(196.00f, "G"),
                        new GuitarString(246.96f, "B"),
                        new GuitarString(329.63f, "E")
                    };
                    break;
                case "Русская гитара":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(73.91f, "D"),
                        new GuitarString(98.00f, "G"),
                        new GuitarString(123.48f, "B"),
                        new GuitarString(147.83f, "D"),
                        new GuitarString(196.00f, "G"),
                        new GuitarString(246.96f, "B"),
                        new GuitarString(293.33f, "D")
                    };
                    break;
                case "Бразильская гитара":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(65.41f, "C"),
                        new GuitarString(82.41f, "E"),
                        new GuitarString(110.00f, "A"),
                        new GuitarString(147.83f, "D"),
                        new GuitarString(196.00f, "G"),
                        new GuitarString(246.96f, "B"),
                        new GuitarString(329.63f, "E")
                    };
                    break;
                default:
                    tuningNotes = new GuitarString[0];
                    break;
            }

            SetupInstrumentLayout(guitar7Layout, tuningNotes);
        }
        private void tuningUkuleleSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string tuning = (string)spinner.GetItemAtPosition(e.Position);

            switch (tuning)
            {
                case "Standard":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(392.00f, "G"),
                        new GuitarString(261.63f, "C"),
                        new GuitarString(329.63f, "E"),
                        new GuitarString(440.00f, "A")
                    };
                    break;
                case "Строй D сопрано":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(440.00f, "A"),
                        new GuitarString(293.33f, "D"),
                        new GuitarString(369.99f, "F#"),
                        new GuitarString(493.88f, "B")
                    };
                    break;
                case "Low G":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(196.00f, "G"),
                        new GuitarString(261.63f, "C"),
                        new GuitarString(329.63f, "E"),
                        new GuitarString(440.00f, "A")
                    };
                    break;
                case "Low A":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(220.00f, "A"),
                        new GuitarString(293.33f, "D"),
                        new GuitarString(369.99f, "F#"),
                        new GuitarString(493.88f, "B")
                    };
                    break;
                case "Slack key":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(392.00f, "G"),
                        new GuitarString(261.63f, "C"),
                        new GuitarString(329.63f, "E"),
                        new GuitarString(392.00f, "G")
                    };
                    break;
                case "Строй B (-1)":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(369.99f, "F#"),
                        new GuitarString(246.96f, "B"),
                        new GuitarString(311.13f, "D#"),
                        new GuitarString(415.00f, "G#")
                    };
                    break;
                case "Строй C# (+1)":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(415.30f, "G#"),
                        new GuitarString(277.18f, "C#"),
                        new GuitarString(349.23f, "F4"),
                        new GuitarString(466.16f, "A#")
                    };
                    break;
                default:
                    tuningNotes = new GuitarString[0]; // Пустой массив, если настройка не определена
                    break;
            }

            SetupInstrumentLayout(ukuleleLayout, tuningNotes);
        }
        private void tuningCelloSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string tuning = (string)spinner.GetItemAtPosition(e.Position);

            switch (tuning)
            {
                case "Standard":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(65.41f, "C"),
                        new GuitarString(98.00f, "G"),
                        new GuitarString(147.83f, "D"),
                        new GuitarString(220.00f, "A")
                    };
                    break;
                case "Золтан Кодай":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(61.74f, "B"),
                        new GuitarString(92.50f, "F#"),
                        new GuitarString(147.83f, "D"),
                        new GuitarString(220.00f, "A")
                    };
                    break;
                default:
                    tuningNotes = new GuitarString[0]; // Пустой массив, если настройка не определена
                    break;
            }

            SetupInstrumentLayout(celloLayout, tuningNotes);
        }
        private void tuningBasSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string tuning = (string)spinner.GetItemAtPosition(e.Position);

            switch (tuning)
            {
                case "Standard":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(41.21f, "E"),
                        new GuitarString(55.00f, "A"),
                        new GuitarString(73.91f, "D"),
                        new GuitarString(98.00f, "G")
                    };
                    break;
                case "На пол тона ниже":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(38.88f, "D#"),
                        new GuitarString(51.90f, "G#"),
                        new GuitarString(63.30f, "C#"),
                        new GuitarString(92.50f, "F#")
                    };
                    break;
                case "На тон ниже":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(36.95f, "D"),
                        new GuitarString(49.00f, "G"),
                        new GuitarString(65.41f, "C"),
                        new GuitarString(87.31f, "F")
                    };
                    break;
                case "Drop D":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(36.95f, "D"),
                        new GuitarString(55.00f, "A"),
                        new GuitarString(73.91f, "D"),
                        new GuitarString(98.00f, "G")
                    };
                    break;
                case "Drop C":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(32.70f, "C"),
                        new GuitarString(49.00f, "G"),
                        new GuitarString(65.41f, "C"),
                        new GuitarString(87.31f, "F")
                    };
                    break;
                case "Drop C#":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(34.65f, "C#"),
                        new GuitarString(51.90f, "G#"),
                        new GuitarString(69.30f, "C#"),
                        new GuitarString(92.50f, "F#")
                    };
                    break;
                case "На пол тона выше":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(43.65f, "F"),
                        new GuitarString(58.26f, "A#"),
                        new GuitarString(77.78f, "D#"),
                        new GuitarString(103.80f, "G#")
                    };
                    break;
                case "На тон выше":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(46.25f, "F#"),
                        new GuitarString(61.74f, "B"),
                        new GuitarString(82.41f, "E"),
                        new GuitarString(110.00f, "A")
                    };
                    break;
                default:
                    tuningNotes = new GuitarString[0]; // Пустой массив, если настройка не определена
                    break;
            }

            SetupInstrumentLayout(basLayout, tuningNotes);
        }
        private void tuningGuitarSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string tuning = (string)spinner.GetItemAtPosition(e.Position);

            switch (tuning)
            {
                case "Standard":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(82.41f, "E"),
                        new GuitarString(110.00f, "A"),
                        new GuitarString(146.83f, "D"),
                        new GuitarString(196.00f, "G"),
                        new GuitarString(246.96f, "B"),
                        new GuitarString(329.63f, "E")
                    };
                    break;
                case "Drop D":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(73.91f, "D"),
                        new GuitarString(110.00f, "A"),
                        new GuitarString(146.83f, "D"),
                        new GuitarString(196.00f, "G"),
                        new GuitarString(246.96f, "B"),
                        new GuitarString(329.63f, "E")
                    };
                    break;
                case "Dsus 4":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(73.91f, "D"),
                        new GuitarString(110.00f, "A"),
                        new GuitarString(146.83f, "D"),
                        new GuitarString(196.00f, "G"),
                        new GuitarString(220.00f, "A"),
                        new GuitarString(293.33f, "D")
                    };
                    break;
                case "Asus 2":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(82.41f, "E"),
                        new GuitarString(110.00f, "A"),
                        new GuitarString(123.48f, "B"),
                        new GuitarString(164.81f, "E"),
                        new GuitarString(220.00f, "A"),
                        new GuitarString(329.63f, "E")
                    };
                    break;
                case "Asus 4":
                    tuningNotes = new GuitarString[]
                    {
                        new GuitarString(82.41f, "E"),
                        new GuitarString(110.00f, "A"),
                        new GuitarString(146.83f, "D"),
                        new GuitarString(164.81f, "E"),
                        new GuitarString(220.00f, "A"),
                        new GuitarString(329.63f, "E")
                    };
                    break;
            case "Drop C":
                tuningNotes = new GuitarString[]
                {
                    new GuitarString(65.41f, "C"),
                    new GuitarString(98.00f, "G"),
                    new GuitarString(131.81f, "C"),
                    new GuitarString(174.91f, "F"),
                    new GuitarString(220.00f, "A"),
                    new GuitarString(293.70f, "D")
                };
                break;
            case "На пол тона ниже":
                tuningNotes = new GuitarString[]
                {
                    new GuitarString(77.78f, "D#"),
                    new GuitarString(103.80f, "G#"),
                    new GuitarString(138.59f, "C#"),
                    new GuitarString(185.00f, "F#"),
                    new GuitarString(233.08f, "A#"),
                    new GuitarString(311.13f, "D#")
                };
                break;
            case "Drop C#":
                tuningNotes = new GuitarString[]
                {
                    new GuitarString(69.30f, "C#"),
                    new GuitarString(103.80f, "G#"),
                    new GuitarString(138.59f, "C#"),
                    new GuitarString(185.00f, "F#"),
                    new GuitarString(233.08f, "A#"),
                    new GuitarString(311.13f, "D#")
                };
                break;
            case "На тон ниже":
                tuningNotes = new GuitarString[]
                {
                    new GuitarString(73.42f, "D"),
                    new GuitarString(98.00f, "G"),
                    new GuitarString(130.82f, "C"),
                    new GuitarString(174.62f, "F"),
                    new GuitarString(220.00f, "A"),
                    new GuitarString(293.66f, "D")
                };
                break;
            case "Open D":
                tuningNotes = new GuitarString[]
                {
                    new GuitarString(73.91f, "D"),
                    new GuitarString(110.00f, "A"),
                    new GuitarString(146.83f, "D"),
                    new GuitarString(185.00f, "F#"),
                    new GuitarString(220.00f, "A"),
                    new GuitarString(293.70f, "D")
                };
                break;
                default:
                    tuningNotes = new GuitarString[0]; // Empty array if tuning is not defined
                    break;
            }

            SetupInstrumentLayout(guitarLayout, tuningNotes);
        }
        private void tuningViolinSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            
            tuningNotes = new GuitarString[]
            {
                new GuitarString(329.63f, "E"),
                new GuitarString(440.00f, "A"),
                new GuitarString(293.66f, "D"),
                new GuitarString(196.00f, "G")
            };
            SetupInstrumentLayout(violinLayout, tuningNotes);
        }
        
        public class DrawingView : View
         {
             private Paint _paint;
             public float Frequency;
             public string Note = "";
             private int _circleRadius = 20;
             private int _rectSize = 40;
             private Paint _circle;
             public float minFrequency;
             public float maxFrequency;
             private System.Timers.Timer _timer;
             private Paint _rect;

             public DrawingView(Context context) : base(context)
             {
                 Initialize();
             }

             private void Initialize()
             {
                 _paint = new Paint
                 {
                     Color = Color.Rgb(255,255,255),
                     StrokeWidth = 5,
                     AntiAlias = true
                 };

                 _rect = new Paint
                 {
                     Color = Color.Rgb(0,191,255),
                     AntiAlias = true
                 };
                 
                 _circle = new Paint
                 {
                     Color = Color.Rgb(0,191,255),
                     AntiAlias = true
                 };
                 _timer = new System.Timers.Timer(30);
                 _timer.Elapsed += OnTimerElapsed;
                 _timer.AutoReset = true;
                 _timer.Start();
             }
             
             private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
             {
                 PostInvalidate();
             }


             public static Color InterpolateColor(double value)
             {
                 Color green = Color.Green;
                 Color red = Color.Red;

                 if (value > 0.5)
                 {
                     value = 1 - value;
                 }
                 
                 value /= 0.5;

                 int r = (int)(red.R * (1 - value) + green.R * value);
                 int g = (int)(red.G * (1 - value) + green.G * value);
                 int b = (int)(red.B * (1 - value) + green.B * value);
        
                 return Color.Rgb(r, g, b);
             }
             
             public static (float frequency, string note) FindNearestFrequency(Double parsedFrequency)
             {
                 Double minDifference = float.MaxValue;
                 float nearestFrequency = 0;
                 string nearestNote = "";

                 foreach (var guitarString in tuningNotes)
                 {
                     Double difference = Math.Abs(guitarString.Frequency - parsedFrequency);
                     if (difference < minDifference)
                     {
                         minDifference = difference;
                         nearestFrequency = guitarString.Frequency;
                         nearestNote = guitarString.Note;
                     }
                 }

                 return (nearestFrequency, nearestNote);
             }
             
             protected override void OnDraw(Canvas canvas)
             {
                 base.OnDraw(canvas);
                 
                 var screenWidth = Width;
                 var screenHeight = Height;
                 double parsedFrequency = 0;
                 lock (lockObject)
                 {
                     parsedFrequency = freq;
                 }

                 /////////////////// for auto
                 // if (true)
                 // {
                 //     var find =FindNearestFrequency(parsedFrequency);
                 //     Frequency = find.frequency;
                 //     Note = find.note;
                 // }
                /////////////////////////////////////
                 var percentage = (parsedFrequency - minFrequency) / (maxFrequency - minFrequency);

                 _rect.Color = InterpolateColor(percentage);
                 _circle.Color = InterpolateColor(percentage);
                 var circleX = (int)(screenWidth * percentage);
                 
                 var lineX = screenWidth / 2;
                 canvas.DrawLine(lineX, 60, lineX, screenHeight-50, _paint);
                 
                 canvas.DrawLine(lineX - _circleRadius, 60 + _circleRadius*2, lineX - _circleRadius, screenHeight-50 - _circleRadius*2, _paint);
                 canvas.DrawLine(lineX + _circleRadius, 60 + _circleRadius*2, lineX + _circleRadius, screenHeight-50 - _circleRadius*2, _paint);

                 
                 canvas.DrawRect(circleX - _circleRadius, screenHeight/2 - _circleRadius, circleX + _circleRadius,screenHeight/2 + _circleRadius, _rect);
                 //canvas.DrawCircle(circleX, screenHeight / 2, _circleRadius, _circle);
                 
                 var borderLeft = "-20";
                 var borderRight = "+20";
                 
                 _paint.TextSize = 40;
                 canvas.DrawText(borderLeft, 20, _paint.TextSize, _paint);
                 canvas.DrawText(borderRight, Width-10-70, _paint.TextSize, _paint);
                 
                 var frequencyText = "Frequency: " + parsedFrequency.ToString("F2");
                 canvas.DrawText(frequencyText, Width/2 -140, _paint.TextSize, _paint);
                 var note = "Note " + Note + " Frequency " + Frequency.ToString("F2"); 
                 canvas.DrawText(note, Width/2 -200, screenHeight-5, _paint);
             }

         }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
         {
             base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
             if (requestCode == RequestRecordAudioPermissionCode)
             {
                 if (!(grantResults.Length > 0 && grantResults[0] == Permission.Granted))
                 {
                     Toast.MakeText(this, "Permission denied to record audio", ToastLength.Short)?.Show();
                 }
             }
         }
        private void SoundCapturer_FrequencyDetected(object sender, FrequencyDetectedEventArgs e)
         {
             RunOnUiThread(() =>
             {
                 lock (lockObject)
                 {
                     freq = e.Freq;          
                 }
             });
         }

    }
}

// namespace Tunerv1._0
// {
//     [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
//     public class MainActivity : AppCompatActivity
//     {
//         private SoundCapturer _soundCapturer;
//         private TextView _textView;
//         private const int RequestRecordAudioPermissionCode = 100;
//         private static double freq;
//         private static object lockObject = new object(); 
//         
//         private int stringButtonsLayoutIndex;
//         private ViewGroup mainView;
//         
//         protected override void OnCreate(Bundle savedInstanceState)
//         {
//             base.OnCreate(savedInstanceState);
//             SetContentView(Resource.Layout.activity_main);
//             RelativeLayout guitarButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.guitarButtonsLayout);
//             View guitarButtonsView = guitarButtonsLayout;
//             mainView = (ViewGroup)guitarButtonsView.Parent;
//             stringButtonsLayoutIndex = mainView.IndexOfChild(guitarButtonsLayout);
//             Spinner instrumentSpinner = FindViewById<Spinner> (Resource.Id.instrumentsSpinner);
//             
//             instrumentSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (instrumentSpinner_ItemSelected);
//             var instrumentsAdapter = ArrayAdapter.CreateFromResource (
//                 this, Resource.Array.instruments_array, Android.Resource.Layout.SimpleSpinnerItem);
//
//             instrumentsAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
//             instrumentSpinner.Adapter = instrumentsAdapter;
//             base.OnCreate(savedInstanceState);
//             //SetContentView(new DrawingView(this));
//             
//             // RelativeLayout guitarButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.guitarButtonsLayout);
//             // View guitarButtonsView = guitarButtonsLayout;
//             // //mainView = (ViewGroup)guitarButtonsView.Parent;
//             // //stringButtonsLayoutIndex = mainView.IndexOfChild(guitarButtonsLayout);
//             // //Spinner instrumentSpinner = FindViewById<Spinner> (Resource.Id.instrumentsSpinner);
//             //
//             // // instrumentSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (instrumentSpinner_ItemSelected);
//             // // var instrumentsAdapter = ArrayAdapter.CreateFromResource (
//             // //     this, Resource.Array.instruments_array, Android.Resource.Layout.SimpleSpinnerItem);
//             // //
//             // // instrumentsAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
//             // // instrumentSpinner.Adapter = instrumentsAdapter;
//             //
//             // _textView = FindViewById<TextView>(Resource.Id.textView);
//             // _soundCapturer = new SoundCapturer();
//             // _soundCapturer.FrequencyDetected += SoundCapturer_FrequencyDetected;
//             //
//             // if (CheckSelfPermission(Manifest.Permission.RecordAudio) != Permission.Granted)
//             // {
//             //     RequestPermissions(new [] { Manifest.Permission.RecordAudio }, RequestRecordAudioPermissionCode);
//             // }
//             
//             //_soundCapturer.StartCapture();
//             
//             
//             // base.OnCreate(savedInstanceState);
//             // SetContentView(Resource.Layout.activity_main);
//             // RelativeLayout guitarButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.guitarButtonsLayout);
//             // View guitarButtonsView = guitarButtonsLayout;
//             // mainView = (ViewGroup)guitarButtonsView.Parent;
//             // stringButtonsLayoutIndex = mainView.IndexOfChild(guitarButtonsLayout);
//             // Spinner instrumentSpinner = FindViewById<Spinner> (Resource.Id.instrumentsSpinner);
//             //
//             // instrumentSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (instrumentSpinner_ItemSelected);
//             // var instrumentsAdapter = ArrayAdapter.CreateFromResource (
//             //     this, Resource.Array.instruments_array, Android.Resource.Layout.SimpleSpinnerItem);
//             //
//             // instrumentsAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
//             // instrumentSpinner.Adapter = instrumentsAdapter;
//             
//         }
//         
//         
//         private void instrumentSpinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
//         {
//             // Spinner spinner = (Spinner)sender;
//             // string instrument = (string)spinner.GetItemAtPosition(e.Position);
//             //
//             // switch (instrument)
//             // {
//             //     case "Guitar":
//             //         View guitarLayout = LayoutInflater.Inflate(Resource.Layout.guitar_buttons_layout, mainView, false);
//             //         RelativeLayout guitarButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.guitarButtonsLayout);
//             //         
//             //         mainView.RemoveViewAt(stringButtonsLayoutIndex);
//             //
//             //         mainView.AddView(guitarLayout, stringButtonsLayoutIndex);
//             //         break;
//             //     case "Violin":
//             //         View violinLayout = LayoutInflater.Inflate(Resource.Layout.violin_buttons_layout, mainView, false);
//             //         RelativeLayout violinButtonsLayout = FindViewById<RelativeLayout>(Resource.Id.guitarButtonsLayout);
//             //         
//             //         mainView.RemoveViewAt(stringButtonsLayoutIndex);
//             //
//             //         mainView.AddView(violinLayout, stringButtonsLayoutIndex);
//             //         break;
//             // }
//         }
//         
//         public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
//         {
//             base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
//             if (requestCode == RequestRecordAudioPermissionCode)
//             {
//                 if (!(grantResults.Length > 0 && grantResults[0] == Permission.Granted))
//                 {
//                     Toast.MakeText(this, "Permission denied to record audio", ToastLength.Short)?.Show();
//                 }
//             }
//         }
//         private void SoundCapturer_FrequencyDetected(object sender, FrequencyDetectedEventArgs e)
//         {
//             RunOnUiThread(() =>
//             {
//                 lock (lockObject)
//                 {
//                     freq = e.Freq;          
//                 }
//             });
//         }
//         
//         public class DrawingView : View
//         {
//             private Paint _paint;
//             private float _frequency = 110.63f; // Пример частоты
//             private int _circleRadius = 10; // Радиус кружка
//             private Paint _circle;
//             float minFrequency = 329.63f - 40;
//             float maxFrequency = 329.63f + 40;
//             private System.Timers.Timer _timer;
//
//             public DrawingView(Context context) : base(context)
//             {
//                 Initialize();
//             }
//
//             private void Initialize()
//             {
//                 _paint = new Paint
//                 {
//                     Color = Color.Rgb(155,45,48),
//                     StrokeWidth = 5,
//                     AntiAlias = true
//                 };
//
//                 _circle = new Paint
//                 {
//                     Color = Color.Rgb(0,0,255),
//                     AntiAlias = true
//                 };
//                 _timer = new System.Timers.Timer(30);
//                // _timer.Elapsed += OnTimerElapsed;
//                 _timer.AutoReset = true; // Запускать таймер снова после каждого срабатывания
//                 _timer.Start();
//             }
//             
//             private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
//             {
//                 // Перерисовываем элемент управления
//                 PostInvalidate();
//             }
//
//
//             protected override void OnDraw(Canvas canvas)
//             {
//                 base.OnDraw(canvas);
//                 
//                 var screenWidth = Width / 2;
//                 var screenHeight = Height / 2;
//                 double parsedFrequency = 0;
//                 lock (lockObject)
//                 {
//                     parsedFrequency = freq;
//                 }
//
//                 var percentage = (parsedFrequency - minFrequency) / (maxFrequency - minFrequency);
//                 
//                 var circleX = (int)(screenWidth * percentage);
//                 
//                 var lineX = screenWidth / 2;
//                 canvas.DrawLine(lineX, 0, lineX, screenHeight, _paint);
//                 
//                 canvas.DrawCircle(circleX, screenHeight / 2, _circleRadius, _circle);
//                 var frequencyText = "Frequency: " + parsedFrequency.ToString("F2");
//                 _paint.TextSize = 100;
//                 canvas.DrawText(frequencyText, 0, _paint.TextSize, _paint);
//                 var note = "note" + _frequency.ToString("F2"); 
//                 canvas.DrawText(note, 100, _paint.TextSize+100, _paint);
//             }
//         }
//         
//     }
// }
/*        private void SetupGuitarLayout(View guitarLayout)
        {
            Button guitarButtonE = guitarLayout.FindViewById<Button>(Resource.Id.buttonE);
            guitarButtonE.Click += (sender, e) =>
            {
                drawingView.Frequency = 82.41f;
                drawingView.minFrequency = 82.41f - 40;
                drawingView.maxFrequency = 82.41f + 40;
                drawingView.Note = "E";

            };
            
            Button guitarButtonA = guitarLayout.FindViewById<Button>(Resource.Id.buttonA);
            guitarButtonA.Click += (sender, e) =>
            {
                drawingView.Frequency = 110.00f;
                drawingView.minFrequency = 110.00f - 40;
                drawingView.maxFrequency = 110.00f + 40;
                drawingView.Note = "A";

            };
            
            Button guitarButtonD = guitarLayout.FindViewById<Button>(Resource.Id.buttonD);
            guitarButtonD.Click += (sender, e) =>
            {
                drawingView.Frequency = 147.83f;
                drawingView.minFrequency = 147.83f - 40;
                drawingView.maxFrequency = 147.83f + 40;
                drawingView.Note = "D";

            };
            
            Button guitarButtonG = guitarLayout.FindViewById<Button>(Resource.Id.buttonG);
            guitarButtonG.Click += (sender, e) =>
            {
                drawingView.Frequency = 196.00f;               
                drawingView.minFrequency = 196.00f - 40;
                drawingView.maxFrequency = 196.00f + 40;
                drawingView.Note = "G";

            };
            
            Button guitarButtonB = guitarLayout.FindViewById<Button>(Resource.Id.buttonB);
            guitarButtonB.Click += (sender, e) =>
            {
                drawingView.Frequency = 246.96f;
                drawingView.minFrequency = 246.96f - 40;
                drawingView.maxFrequency = 246.96f + 40;
                drawingView.Note = "B";

            };
            
            Button guitarButtonE2 = guitarLayout.FindViewById<Button>(Resource.Id.buttonE2);
            guitarButtonE2.Click += (sender, e) =>
            {
                drawingView.Frequency = 329.63f;
                drawingView.minFrequency = 329.63f - 40;
                drawingView.maxFrequency = 329.63f + 40;
                drawingView.Note = "E";

            };
        }
        private void SetupViolinLayout(View violinLayout)
        {
            Button violinButtonE = violinLayout.FindViewById<Button>(Resource.Id.buttonE);
            violinButtonE.Click += (sender, e) =>
            {
                drawingView.Frequency = 329.63f;
                drawingView.minFrequency = 82.41f - 40;
                drawingView.maxFrequency = 82.41f + 40;
                drawingView.Note = "E";

            };
            
            Button violinButtonA = violinLayout.FindViewById<Button>(Resource.Id.buttonA);
            violinButtonA.Click += (sender, e) =>
            {
                drawingView.Frequency = 440.00f;
                drawingView.minFrequency = 440.00f - 40;
                drawingView.maxFrequency = 440.00f + 40;
                drawingView.Note = "A";

            };
            
            Button violinButtonD = violinLayout.FindViewById<Button>(Resource.Id.buttonD);
            violinButtonD.Click += (sender, e) =>
            {
                drawingView.Frequency = 293.66f;
                drawingView.minFrequency = 293.66f - 40;
                drawingView.maxFrequency = 293.66f + 40;
                drawingView.Note = "D";

            };
            
            Button violinButtonG = violinLayout.FindViewById<Button>(Resource.Id.buttonG);
            violinButtonG.Click += (sender, e) =>
            {
                drawingView.Frequency = 196.00f;               
                drawingView.minFrequency = 196.00f - 40;
                drawingView.maxFrequency = 196.00f + 40;
                drawingView.Note = "G";

            };
        }
        */