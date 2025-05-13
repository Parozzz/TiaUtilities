using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TiaUtilities.Javascript.ErrorReporting;
using TiaUtilities.Javascript.FCTB;
using TiaXmlReader.Utility;
using Timer = System.Windows.Forms.Timer;

namespace TiaUtilities.Javascript
{
    class VisualError
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string? Description { get; set; }
    }

    public class FCTBErrorVisualizer
    {
        private const int SHOW_ERROR_DELAY_AFTER_TEXT_CHANGED = 1250; //ms
        private static readonly Style SAME_WORDS_STYLE = new MarkerStyle(new SolidBrush(Color.FromArgb(65, Color.Green)));
        private static readonly Style LINE_ERROR_WAVY_STYLE = new CustomWavyLineStyle(Color.FromArgb(255, Color.DarkRed), Color.FromArgb(125, Color.MediumVioletRed));

        private readonly FastColoredTextBox textBox;
        private readonly ErrorReporter errorReporter;

        private VisualError? currentError;
        private bool haltError; //This is required to avoid having the wavy style contantly flash on screen. After changing text, give some time before accetting new text!

        public FCTBErrorVisualizer(FastColoredTextBox textBox, ErrorReporter errorReporter) 
        {
            this.textBox = textBox;
            this.errorReporter = errorReporter;
        }

        public void Init()
        {
            //Highligh same word if something is selected.
            this.textBox.SelectionChangedDelayed += SelectionChangedDelayed;
            this.textBox.ToolTipNeeded += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.ToolTipTitle) || !string.IsNullOrEmpty(args.ToolTipText))
                {
                    return;
                }

                if (currentError != null)
                {
                    var errorLine = currentError.Line;
                    var errorColumn = currentError.Column;

                    var lineEnd = this.textBox.GetLine(errorLine).End;

                    var place = args.Place;
                    if (place.iLine == errorLine && (errorColumn >= lineEnd.iChar || place.iChar >= errorColumn))
                    {  //Display the error after the error column ONLY if valid, otherwise while hovering the whole line!
                        args.ToolTipTitle = "Error";
                        args.ToolTipText = currentError.Description;
                        args.ToolTipIcon = ToolTipIcon.Error;
                    }
                }
            };

            var resetHaltErrorTimer = new Timer() { Interval = SHOW_ERROR_DELAY_AFTER_TEXT_CHANGED };
            resetHaltErrorTimer.Tick += (sender, args) =>
            {
                resetHaltErrorTimer.Stop();
                haltError = false;
            };

            this.textBox.TextChanged += (sender, args) =>
            {
                ClearError();
                haltError = true;

                resetHaltErrorTimer.Stop(); //This reset the timer.
                resetHaltErrorTimer.Start();
            };

            this.errorReporter.CompleteEvent += (sender, args) =>
            {
                var error = args.ErrorList.Count == 0 ? null : args.ErrorList[0];
                ErrorReportingComplete(error);
            };
        }

        private void ErrorReportingComplete(ReportedError? reportedError)
        {
            if (haltError)
            {
                return;
            }

            if (reportedError == null)
            {
                if (currentError != null)
                {
                    ClearError();
                }
            }
            else
            {
                var newError = new VisualError()
                {
                    Line = (int) reportedError.Line,
                    Column = (int) reportedError.Column,
                    Description = reportedError.Description,
                };

                if (AreErrorLimitValid(newError) && Utils.AreDifferentObject(currentError, newError))
                {
                    ClearError();

                    currentError = newError;
                    UpdateCurrentError();
                }
            }
        }

        private void UpdateCurrentError()
        {
            if (currentError == null || !AreErrorLimitValid(currentError))
            {
                return;
            }

            var lineRange = this.textBox.GetLine(currentError.Line);

            var start = lineRange.Start;
            var end = lineRange.End;
            if (currentError.Column >= 0 && currentError.Column < lineRange.Length)
            {
                lineRange.Start = new Place(currentError.Column, currentError.Line);
                lineRange.End = end;
            }

            lineRange.SetStyle(LINE_ERROR_WAVY_STYLE);
        }

        private void ClearError()
        {
            this.textBox.Range.ClearStyle(LINE_ERROR_WAVY_STYLE);
            currentError = null;
        }

        private bool AreErrorLimitValid(VisualError error)
        {
            return error.Line >= 0 && error.Line < this.textBox.LinesCount && error.Column >= 0 && !string.IsNullOrEmpty(error.Description);
        }

        private void SelectionChangedDelayed(object? sender, EventArgs args)
        {
            try
            {
                this.textBox.VisibleRange.ClearStyle(SAME_WORDS_STYLE);

                //get fragment around caret
                string text = this.textBox.Selection.Text;
                if (text.Length < 2)
                {
                    return;
                }

                //highlight same words
                var ranges = this.textBox.VisibleRange.GetRanges(Regex.Escape(text)).ToArray();
                if (ranges.Length > 1)
                {
                    foreach (var r in ranges)
                    {
                        r.SetStyle(SAME_WORDS_STYLE);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }

    }
}
