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
        public static int[,] CreateMinesArray(int minesCount, int ySize = 10, int xSize = 15)
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
        public static void PlayNote(int noteNumber, int deltaTime = 40)
        {
            noteNumber = noteNumber > 127 ? 127 : noteNumber;
            var noteOn = new NoteOnEvent((SevenBitNumber)noteNumber, (SevenBitNumber)127);
            var noteOff = new NoteOffEvent((SevenBitNumber)noteNumber, (SevenBitNumber)0) { DeltaTime = deltaTime};
            var midiFile = new MidiFile(new TrackChunk(new SetTempoEvent(60000)), new TrackChunk( noteOn, noteOff));
            var playback = midiFile.GetPlayback(outputDevice);
            playback.Play();
        }
        public static void PlayGlissando(int startNote, int relativeSpan, int deltaTime = 20)
        {
            startNote = startNote > 100 ? (byte)100 : startNote;
            int endNote = startNote + relativeSpan;
            endNote = endNote > 127 ? (byte)127 : endNote;
            var notes = new List<MidiEvent>();
            int step = startNote > endNote ? -1 : 1;
            for (int i = startNote; i != endNote; i += step)
            {
                notes.Add(new NoteOnEvent((SevenBitNumber)i, (SevenBitNumber)127));
                notes.Add(new NoteOffEvent((SevenBitNumber)i, (SevenBitNumber)0) { DeltaTime = deltaTime });
            }
            var midiFile = new MidiFile(new TrackChunk(new SetTempoEvent(60000)), new TrackChunk(notes.ToArray()));
            var playback = midiFile.GetPlayback(outputDevice);
            playback.Play();
        }
        public static void PlayGlissandoPentatonic(int startNote, int relativeSpan, int deltaTime = 20)
        {
            startNote = startNote > 100 ? (byte)100 : startNote;
            int endNote = startNote + relativeSpan;
            var notes = new List<MidiEvent>();
            int step = startNote > endNote ? -1 : 1;
            for (int i = startNote; i != endNote; i += step)
            {
                if (i % 12 == 0 || i % 12 == 2 || i % 12 == 4 || i % 12 == 7 || i % 12 == 9)
                {
                    notes.Add(new NoteOnEvent((SevenBitNumber)i, (SevenBitNumber)127));
                    notes.Add(new NoteOffEvent((SevenBitNumber)i, (SevenBitNumber)0) { DeltaTime = deltaTime });
                }
            }
            var midiFile = new MidiFile(new TrackChunk(new SetTempoEvent(60000)), new TrackChunk(notes.ToArray()));
            var playback = midiFile.GetPlayback(outputDevice);
            playback.Play();
        }
        public static void PlayChordMajor(int startNote = 60, int deltaTime = 200)
        {
            startNote = startNote > 100 ? (byte)100 : startNote;
            MidiEvent[] notes =
            [
                new NoteOnEvent((SevenBitNumber)startNote, (SevenBitNumber)127),
                new NoteOnEvent((SevenBitNumber)(startNote + 4), (SevenBitNumber)127),
                new NoteOnEvent((SevenBitNumber)(startNote + 7), (SevenBitNumber)127),
                new NoteOffEvent((SevenBitNumber)startNote, (SevenBitNumber)0) { DeltaTime = deltaTime },
                new NoteOffEvent((SevenBitNumber)(startNote + 4), (SevenBitNumber)0) { DeltaTime = deltaTime },
                new NoteOffEvent((SevenBitNumber)(startNote + 7), (SevenBitNumber)0) { DeltaTime = deltaTime },
            ];
            var midiFile = new MidiFile(new TrackChunk(new SetTempoEvent(60000)), new TrackChunk(notes));
            var playback = midiFile.GetPlayback(outputDevice);
            playback.Play();
        }
        public static void PlayChordMinor(int startNote = 60, int deltaTime = 200)
        {
            startNote = startNote > 100 ? (byte)100 : startNote;
            MidiEvent[] notes =
            [
                new NoteOnEvent((SevenBitNumber)startNote, (SevenBitNumber)127),
                new NoteOnEvent((SevenBitNumber)(startNote + 3), (SevenBitNumber)127),
                new NoteOnEvent((SevenBitNumber)(startNote + 7), (SevenBitNumber)127),
                new NoteOffEvent((SevenBitNumber)startNote, (SevenBitNumber)0) { DeltaTime = deltaTime },
                new NoteOffEvent((SevenBitNumber)(startNote + 3), (SevenBitNumber)0) { DeltaTime = deltaTime },
                new NoteOffEvent((SevenBitNumber)(startNote + 7), (SevenBitNumber)0) { DeltaTime = deltaTime },
            ];          
            var midiFile = new MidiFile(new TrackChunk(new SetTempoEvent(60000)), new TrackChunk(notes));
            var playback = midiFile.GetPlayback(outputDevice);
            playback.Play();
        }
        public static void PlayWinChordProgression(int startNote = 60, int deltaTime = 100)
        {
            PlayChordMajor(startNote, deltaTime * 2);
            PlayChordMinor(startNote + 4, deltaTime * 2);
            PlayChordMinor(startNote - 3, deltaTime);
            PlayChordMajor(startNote + 5, deltaTime);
            PlayChordMajor(startNote + 7, deltaTime * 3);
        }
    }
}
