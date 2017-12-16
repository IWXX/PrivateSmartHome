﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace FacialRecognizedDoorClient.Helpers
{
    class SpeechHelper : IDisposable
    {
        private MediaElement mediaElement;
        private SpeechSynthesizer synthesizer;

        /// <summary>
        /// Accepts a MediaElement that should be placed on whichever page user is on when text is read by SpeechHelper.
        /// Initializes SpeechSynthesizer.
        /// </summary>
        public SpeechHelper(MediaElement media)
        {
            mediaElement = media;
            synthesizer = new SpeechSynthesizer();
            synthesizer.Options.SpeakingRate = 0.8;
      
        }

        /// <summary>
        /// Synthesizes passed through text as audio and plays speech through the MediaElement first sent through.
        /// </summary>
        public async Task Read(string text)
        {
            if (mediaElement != null && synthesizer != null)
            {
                var stream = await synthesizer.SynthesizeTextToStreamAsync(text);
                mediaElement.AutoPlay = true;
                mediaElement.SetSource(stream, stream.ContentType);
                mediaElement.Play();
            }
        }

        /// <summary>
        /// Disposes of IDisposable type SpeechSynthesizer
        /// </summary>
        public void Dispose()
        {
            synthesizer.Dispose();
        }
    }
}
