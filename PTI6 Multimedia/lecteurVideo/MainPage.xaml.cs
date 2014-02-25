using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace lecteurVideo
{
	public partial class MainPage : UserControl
	{
		private DispatcherTimer timer_barre;
        private DispatcherTimer timer;
		private bool changerTemps;
        private double hauteurApplication;
		private double largeurApplication;
		
		// Chargement de l'application
		public MainPage()
		{
			// Requis pour initialiser des variables
			InitializeComponent();
		}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Indique si il faut changer le timing
            changerTemps = false;

            // Taille de l'appli (pour le passage en plein ecran)
            hauteurApplication = this.Height;
            largeurApplication = this.Width;

            timer = new DispatcherTimer();
            timer_barre = new DispatcherTimer();
			
			SCR_Volume.ValueChanged +=new System.Windows.RoutedPropertyChangedEventHandler<double>(SCR_Volume_ValueChanged);
        }
		
		// Ouverture de la vidéo
		private void ME_VideoSon_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
		{
			LBL_Message.Visibility = Visibility.Collapsed;
			
			ME_VideoSon.Play();
			
			BTN_Play.Visibility = Visibility.Collapsed;
			BTN_Pause.Visibility = Visibility.Visible;
			
			// Timer permettant d'actualiser le timing
			timer.Interval = new TimeSpan(0,0,1);
			timer.Tick +=new System.EventHandler(timer_Tick);
			timer.Start();
			
			// Indiquer le temps maximum de la vidéo aux scrollbars
			SCR_Temps.Maximum = ME_VideoSon.NaturalDuration.TimeSpan.TotalSeconds;
			SCR_ChangerTemps.Maximum = ME_VideoSon.NaturalDuration.TimeSpan.TotalSeconds;
			
			// Afficher un 0 si minutes < 0 ou secondes < 0 pour le temps
			string minutes, secondes;
			
			secondes = ME_VideoSon.NaturalDuration.TimeSpan.Seconds.ToString("00");
			minutes = ME_VideoSon.NaturalDuration.TimeSpan.Minutes.ToString("00");
			
			LBL_TempsTotal.Text = minutes + ":" + secondes;
			
			// Evènements
			ME_VideoSon.DownloadProgressChanged +=new System.Windows.RoutedEventHandler(ME_VideoSon_DownloadProgressChanged);
			LayoutRoot.MouseMove +=new System.Windows.Input.MouseEventHandler(LayoutRoot_MouseMove);
			SCR_Temps.MouseEnter +=new System.Windows.Input.MouseEventHandler(SCR_Temps_MouseEnter);
			BTN_Play.Click +=new System.Windows.RoutedEventHandler(BTN_Play_Click);
			BTN_Pause.Click +=new System.Windows.RoutedEventHandler(BTN_Pause_Click);
		}
		
		// Echec de l'ouverture de la vidéo
		private void ME_VideoSon_MediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
		{
            LBL_Message.Text = "Le média n'a pas pu être ouvert";
		}
		
		// Téléchargement de la vidéo
		private void ME_VideoSon_DownloadProgressChanged(object sender, System.Windows.RoutedEventArgs e)
		{
            LNG_Telecharegement.Width = GRD_BarreControle.Width * ME_VideoSon.DownloadProgress;
		}
		
		// Initialisation d'un timer après mouvement de la souris pour pouvoir ensuite masquer la barre du bas
		private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			GRD_BarreControle.Visibility = Visibility.Visible;
			
			timer_barre.Interval = new TimeSpan(0,0,4);
			timer_barre.Tick +=new System.EventHandler(timer_barre_Tick);
			timer_barre.Start();
		}
		
		// Une fois ce timer enclenché, la barre de contrôle disparaît
		private void timer_barre_Tick(object sender, System.EventArgs e)
		{
			GRD_BarreControle.Visibility = Visibility.Collapsed;
		}
		
		private void SCR_Temps_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			changerTemps = false;
			
			SCR_ChangerTemps.Visibility = Visibility.Visible;
			SCR_Temps.Visibility = Visibility.Collapsed;
			SCR_ChangerTemps.Value = ME_VideoSon.Position.TotalSeconds;
			
			SCR_ChangerTemps.MouseMove +=new System.Windows.Input.MouseEventHandler(SCR_ChangerTemps_MouseMove);
			SCR_ChangerTemps.MouseLeave +=new System.Windows.Input.MouseEventHandler(SCR_ChangerTemps_MouseLeave);
		}
		
		private void SCR_ChangerTemps_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			SCR_ChangerTemps.ValueChanged +=new System.Windows.RoutedPropertyChangedEventHandler<double>(SCR_ChangerTemps_ValueChanged);
			changerTemps = true;
		}
		
		private void SCR_ChangerTemps_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if(changerTemps == true)
				ME_VideoSon.Position = new TimeSpan(0, 0, (int)SCR_ChangerTemps.Value);

			changerTemps = false;
		}
		
		private void SCR_ChangerTemps_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			SCR_ChangerTemps.Visibility = Visibility.Collapsed;
			SCR_Temps.Visibility = Visibility.Visible;
			
			SCR_Temps.Value = ME_VideoSon.Position.TotalSeconds;
		}
		
		private void BTN_Play_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ME_VideoSon.Play();
			
			BTN_Play.Visibility = Visibility.Collapsed;
			BTN_Pause.Visibility = Visibility.Visible;
			
			timer.Interval = new TimeSpan(0,0,1);
			timer.Tick +=new System.EventHandler(timer_Tick);
			timer.Start();
		}
		
		private void timer_Tick(object sender, System.EventArgs e)
		{
			string minutes, secondes;
			
			secondes = ME_VideoSon.Position.Seconds.ToString("00");
			minutes = ME_VideoSon.Position.Minutes.ToString("00");
			
			SCR_Temps.Value = ME_VideoSon.Position.TotalSeconds;
			LBL_TempsActuel.Text = minutes + ":" + secondes;
		}

		private void BTN_Pause_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ME_VideoSon.Pause();
			BTN_Play.Visibility = Visibility.Visible;
			BTN_Pause.Visibility = Visibility.Collapsed;
			
			timer.Stop();
		}
		
		private void BTN_PleinEcran_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            if (Application.Current.Host.Content.IsFullScreen == true)
            {
                Application.Current.Host.Content.IsFullScreen = false;

            }
            else
            {
                Application.Current.Host.Content.IsFullScreen = true;
            }

			Application.Current.Host.Content.FullScreenChanged +=new System.EventHandler(Content_FullScreenChanged);
		}
		
		// Evènement qui se déclenche lorsque la vidéo passe en plein écran
		private void Content_FullScreenChanged(object sender, System.EventArgs e)
		{
			// On récupère la taille de l'écran une fois passé en plein écran
			double largeurActuelle = Application.Current.Host.Content.ActualWidth;
   			double hauteurActuelle = Application.Current.Host.Content.ActualHeight;
			
			// On agrandit le lecteur à l'écran entier
			LayoutRootScaleTransform.ScaleX = largeurActuelle / largeurApplication;
			LayoutRootScaleTransform.ScaleY = hauteurActuelle / hauteurApplication; 
		}

		private void SCR_Volume_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			ME_VideoSon.Volume = SCR_Volume.Value;
		}

        private void BTN_OK_Click(object sender, RoutedEventArgs e) // Clic Bouton Ok
        {
            if (CB_Medias.SelectedItem != null) // Voir si un élément est sélectionné
            {
                ME_VideoSon.Stop(); // Stopper le média actuel
				
				// Charger le nouveau média
				ComboBoxItem wItem = (ComboBoxItem)CB_Medias.SelectedItem;
                ChargerMedia(wItem.Content.ToString());
                
				LBL_Message.Visibility = Visibility.Visible;
				LBL_Message.Text = "Ouverture du média...";

                ME_VideoSon.MediaOpened += new System.Windows.RoutedEventHandler(ME_VideoSon_MediaOpened);
                ME_VideoSon.MediaFailed += new System.EventHandler<System.Windows.ExceptionRoutedEventArgs>(ME_VideoSon_MediaFailed);
            
				RECT_Bas.Width = 0;
				
				while(RECT_Bas.Width < LayoutRoot.Width)
				{
					RECT_Bas.Width++;
				}
			}
        }
		
		private void ChargerMedia(string nomVideo)
		{
			ME_VideoSon.Source = new Uri(nomVideo, UriKind.Relative);
		}
	}
}