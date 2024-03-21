using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace Tunerv1._0
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private SoundCapturer _soundCapturer;
        private TextView _textView;
        private const int RequestRecordAudioPermissionCode = 100;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            
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
                _textView.Text = e.Freq.ToString("F2");
            });
        }
    }
}
