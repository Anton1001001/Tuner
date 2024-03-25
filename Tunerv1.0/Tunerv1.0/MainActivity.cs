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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(new DrawingView(this));
            
            _textView = FindViewById<TextView>(Resource.Id.textView);
            _soundCapturer = new SoundCapturer();
            _soundCapturer.FrequencyDetected += SoundCapturer_FrequencyDetected;
            
            if (CheckSelfPermission(Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                RequestPermissions(new [] { Manifest.Permission.RecordAudio }, RequestRecordAudioPermissionCode);
            }
            
            _soundCapturer.StartCapture();
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
        
        public class DrawingView : View
        {
            private Paint _paint;
            private float _frequency = 329.63f; // Пример частоты
            private int _circleRadius = 10; // Радиус кружка
            private Paint _circle;
            float minFrequency = 329.63f - 40;
            float maxFrequency = 329.63f + 40;
            private System.Timers.Timer _timer;

            public DrawingView(Context context) : base(context)
            {
                Initialize();
            }

            private void Initialize()
            {
                _paint = new Paint
                {
                    Color = Color.Rgb(155,45,48),
                    StrokeWidth = 5,
                    AntiAlias = true
                };

                _circle = new Paint
                {
                    Color = Color.Rgb(0,0,255),
                    AntiAlias = true
                };
                _timer = new System.Timers.Timer(30);
                _timer.Elapsed += OnTimerElapsed;
                _timer.AutoReset = true; // Запускать таймер снова после каждого срабатывания
                _timer.Start();
            }
            
            private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                // Перерисовываем элемент управления
                PostInvalidate();
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
                
                var circleX = (int)(screenWidth * percentage);
                
                var lineX = screenWidth / 2;
                canvas.DrawLine(lineX, 0, lineX, screenHeight, _paint);
                
                canvas.DrawCircle(circleX, screenHeight / 2, _circleRadius, _circle);
                var frequencyText = "Frequency: " + parsedFrequency.ToString("F2"); 
                canvas.DrawText(frequencyText, 0, _paint.TextSize, _paint);
            }
        }
    }
}
