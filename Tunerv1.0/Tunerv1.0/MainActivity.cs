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
        private TextView _textView;
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
        
        private void SetupLayout(View layout, Button button, float frequency, string note)
        {
            button.Click += (sender, e) =>
            {
                drawingView.Frequency = frequency;
                drawingView.minFrequency = frequency - 50;
                drawingView.maxFrequency = frequency + 50;
                drawingView.Note = note;
            };
        }

        private void SetupInstrumentLayout(View instrumentLayout, float[] frequencies, string[] notes)
        {
            for (int i = 0; i < frequencies.Length; i++)
            {
                Button button = instrumentLayout.FindViewById<Button>(Resource.Id.button1 + i);
                SetupLayout(instrumentLayout, button, frequencies[i], notes[i]);
                button.Text = notes[i];
            }
        }

        private View guitarLayout;
        private View basLayout;
        private View celloLayout;
        private View violinLayout;
        private View ukuleleLayout;
        private View guitar7Layout;
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
                    SetupInstrumentLayout(guitar7Layout, new float[] { 61.74f, 82.41f, 110.00f, 147.83f, 196.00f, 246.96f, 329.63f }, new string[] { "B", "E", "A", "D", "G", "B", "E"});
                    break;
                case "Drop A":
                    SetupInstrumentLayout(guitar7Layout, new float[] { 55.00f, 82.41f, 110.00f, 147.83f, 196.00f, 246.96f, 329.63f }, new string[] { "A", "E", "A", "D", "G", "B", "E"});
                    break;
                case "Русская гитара":
                    SetupInstrumentLayout(guitar7Layout, new float[] { 73.91f, 98.00f, 123.48f, 147.83f, 196.00f, 246.96f, 293.33f }, new string[] { "D", "G", "B", "D", "G", "B", "D"});
                    break;
                case "Бразильская гитара":
                    SetupInstrumentLayout(guitar7Layout, new float[] { 65.41f, 82.41f, 110.00f, 147.83f, 196.00f, 246.96f, 329.63f }, new string[] { "C", "E", "A", "D", "G", "B", "E"});
                    break;
            }
        }
        
        private void tuningUkuleleSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string tuning = (string)spinner.GetItemAtPosition(e.Position);
            
            switch (tuning)
            {
                case "Standard":
                    SetupInstrumentLayout(ukuleleLayout, new float[] { 392.00f, 261.63f, 329.63f, 440.00f }, new string[] { "G", "C", "E", "A"});
                    break;
                case "Строй D сопрано":
                    SetupInstrumentLayout(ukuleleLayout, new float[] { 440.00f, 293.33f, 369.99f, 493.88f }, new string[] { "A", "D", "F#", "B"});
                    break;
                case "Low G":
                    SetupInstrumentLayout(ukuleleLayout, new float[] { 196.00f, 261.63f, 329.63f, 440.00f }, new string[] { "G", "C", "E", "A"});
                    break;
                case "Low A":
                    SetupInstrumentLayout(ukuleleLayout, new float[] { 220.00f, 293.33f, 369.99f, 493.88f }, new string[] { "A", "D", "F#", "B"});
                    break;
                case "Slack key":
                    SetupInstrumentLayout(ukuleleLayout, new float[] { 392.00f, 261.63f, 329.63f, 392.00f }, new string[] { "G", "C", "E", "G"});
                    break;
                case "Строй B (-1)":
                    SetupInstrumentLayout(ukuleleLayout, new float[] { 369.99f, 246.96f, 311.13f, 415.00f }, new string[] { "F#", "B", "D#", "G#"});
                    break;
                case "Строй C# (+1)":
                    SetupInstrumentLayout(ukuleleLayout, new float[] { 415.30f, 277.18f, 349.23f, 466.16f }, new string[] { "G#", "C#4", "F4", "A#4"});
                    break;
            }
        }
        private void tuningCelloSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string tuning = (string)spinner.GetItemAtPosition(e.Position);


            switch (tuning)
            {
                case"Standard":
                    SetupInstrumentLayout(celloLayout, new float[] { 65.41f, 98.00f, 147.83f, 220.00f }, new string[] { "C", "G", "D", "A"});
                    break;
                case "Золтан Кодай":
                    SetupInstrumentLayout(celloLayout, new float[] { 61.74f, 92.50f, 147.83f, 220.00f }, new string[] { "B", "F#", "D", "A"});
                    break;
            }
        }
        private void tuningBasSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string tuning = (string)spinner.GetItemAtPosition(e.Position);

            
            switch (tuning)
            {
                case"Standard":
                    SetupInstrumentLayout(basLayout, new float[] { 41.21f, 55.00f, 73.91f, 98.00f }, new string[] { "E", "A", "D", "G"});
                    break;
                case"На пол тона ниже":
                    SetupInstrumentLayout(basLayout, new float[] { 38.88f, 51.90f, 63.30f, 92.50f }, new string[] { "D#", "G#", "C#", "F#"});
                    break;
                case"На тон ниже":
                    SetupInstrumentLayout(basLayout, new float[] { 36.95f, 49.00f, 65.41f, 87.31f }, new string[] { "D", "G", "C", "F"});
                    break;
                case"Drop D":
                    SetupInstrumentLayout(basLayout, new float[] { 36.95f, 55.00f, 73.91f, 98.00f }, new string[] { "D", "A", "D", "G"});
                    break;
                case"Drop C":
                    SetupInstrumentLayout(basLayout, new float[] { 32.70f, 49.00f, 65.41f, 87.31f }, new string[] { "C", "G", "C", "F"});
                    break;
                case"Drop C#":
                    SetupInstrumentLayout(basLayout, new float[] { 34.65f, 51.90f, 69.30f, 92.50f }, new string[] { "C#", "G#", "C#", "F#"});
                    break;
                case"На пол тона выше":
                    SetupInstrumentLayout(basLayout, new float[] { 43.65f, 58.26f, 77.78f, 103.80f }, new string[] { "F", "A#", "D#", "G#"});
                    break;
                case"На тон выше":
                    SetupInstrumentLayout(basLayout, new float[] { 46.25f, 61.74f, 82.41f, 110.00f }, new string[] { "F#", "B", "E", "A"});
                    break;
            }
        }
        private void tuningGuitarSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string tuning = (string)spinner.GetItemAtPosition(e.Position);

            switch (tuning)
            {
                case"Standard":
                    SetupInstrumentLayout(guitarLayout, new float[] { 82.41f, 110.00f, 146.83f, 196.00f, 246.96f, 329.63f }, new string[] { "E", "A", "D", "G", "B", "E" });
                    break;
                case"Drop D":
                    SetupInstrumentLayout(guitarLayout, new float[] { 73.91f, 110.00f, 146.83f, 196.00f, 246.96f, 329.63f }, new string[] { "D", "A", "D", "G", "B", "E" });
                    break;
                case"Dsus 4":
                    SetupInstrumentLayout(guitarLayout, new float[] { 73.91f, 110.00f, 146.83f, 196.00f, 220.00f, 293.33f }, new string[] { "D", "A", "D", "G", "A", "D" });
                    break;
                case"Asus 2":
                    SetupInstrumentLayout(guitarLayout, new float[] { 82.41f, 110.00f, 123.48f, 164.81f, 220.00f, 329.63f }, new string[] { "E", "A", "B", "E", "A", "E" });
                    break;                
                case"Asus 4":
                    SetupInstrumentLayout(guitarLayout, new float[] { 82.41f, 110.00f, 146.83f, 164.81f, 220.00f, 329.63f }, new string[] { "E", "A", "D", "E", "A", "E" });
                    break;
                case"Drop C":
                    SetupInstrumentLayout(guitarLayout, new float[] { 65.41f, 98.00f, 131.81f, 174.91f, 220.00f, 293.70f }, new string[] { "C", "G", "C", "F", "A", "D" });
                    break;
                case"На пол тона ниже":
                    SetupInstrumentLayout(guitarLayout, new float[] { 77.78f, 103.80f, 138.59f, 185.00f, 233.08f, 311.13f }, new string[] { "D#", "G#", "C#", "F#", "A#", "D#" });
                    break;
                case"Drop C#":
                    SetupInstrumentLayout(guitarLayout, new float[] { 69.30f, 103.80f, 138.59f, 185.00f, 233.08f, 311.13f }, new string[] { "C#", "G#", "C#", "F#", "A#", "D#" });
                    break;
                case"На тон ниже":
                    SetupInstrumentLayout(guitarLayout, new float[] { 73.42f, 98.00f, 130.82f, 174.62f, 220.00f, 293.66f }, new string[] { "D", "G", "C", "F", "A", "D" });
                    break;
                case"Open D":
                    SetupInstrumentLayout(guitarLayout, new float[] { 73.91f, 110.00f, 146.83f, 185.00f, 220.00f, 293.70f }, new string[] { "D", "A", "D", "F#", "A", "D" });
                    break;
            }
        }        
        private void tuningViolinSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SetupInstrumentLayout(violinLayout, new float[] { 329.63f, 440.00f, 293.66f, 196.00f }, new string[] { "E", "A", "D", "G" });
        }
        
        
        
        public class DrawingView : View
         {
             private Paint _paint;
             public float Frequency; // Пример частоты
             public string Note = ""; // Пример частоты
             private int _circleRadius = 20; // Радиус кружка
             private Paint _circle;
             public float minFrequency;
             public float maxFrequency;
             private System.Timers.Timer _timer;

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

                 var percentage = (parsedFrequency - minFrequency) / (maxFrequency - minFrequency);

                 _circle.Color = InterpolateColor(percentage);
                 var circleX = (int)(screenWidth * percentage);
                 
                 var lineX = screenWidth / 2;
                 canvas.DrawLine(lineX, 60, lineX, screenHeight-50, _paint);
                 canvas.DrawCircle(circleX, screenHeight / 2, _circleRadius, _circle);

                 var borderLeft = "-50";
                 var borderRight = "+50";
                 
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