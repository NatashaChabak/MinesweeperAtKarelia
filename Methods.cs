using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wpf_Karelia
{
    class Methods
    {
        static OutputDevice outputDevice;
        public static int[,] CreateMinesArray(int ySize, int xSize)
        {
            int[,] array = new int[ySize, xSize];
            int minesCount = ySize * xSize / 8;
            Random rnd = new Random();
            while (minesCount > 0)
            {
                int xRandom = rnd.Next(xSize);
                int yRandom = rnd.Next(ySize);
                if (array[yRandom, xRandom] == -1) { continue; }
                array[yRandom, xRandom] = -1;
                minesCount--;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (yRandom + i >= 0 && yRandom + i < ySize && xRandom + j >= 0 && xRandom + j < xSize)
                        {
                            if (array[yRandom + i, xRandom + j] != -1) { array[yRandom + i, xRandom + j]++; }
                        }
                    }
                }
            }
            return array;
        }
        public static void InitializeSound()
        {
            outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
        }
        public static void PlayNote(byte noteNumber)
        {
            var noteOn = new NoteOnEvent(new SevenBitNumber(noteNumber), new SevenBitNumber(noteNumber));
            var midiFile = new MidiFile(new TrackChunk(noteOn));
            var playback = midiFile.GetPlayback(outputDevice);
            playback.Play();
        }
    }
}
