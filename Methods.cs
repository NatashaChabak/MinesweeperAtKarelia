using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections.Generic;

namespace Wpf_Karelia
{
    class Methods
    {
        static OutputDevice outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
        public static int[,] CreateMinesArray(int ySize, int xSize, int minesCount)
        {
            int[,] array = new int[ySize, xSize];
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
        public static void PlayNote(byte noteNumber, int deltaTime = 40)
        {
            noteNumber = noteNumber > 127 ? (byte)127 : noteNumber;
            var noteOn = new NoteOnEvent((SevenBitNumber)noteNumber, (SevenBitNumber)80);
            var noteOff = new NoteOffEvent((SevenBitNumber)noteNumber, (SevenBitNumber)0) { DeltaTime = deltaTime};
            var midiFile = new MidiFile(new TrackChunk(new SetTempoEvent(60000)), new TrackChunk( noteOn, noteOff));
            var playback = midiFile.GetPlayback(outputDevice);
            playback.Play();
        }
    }
}
