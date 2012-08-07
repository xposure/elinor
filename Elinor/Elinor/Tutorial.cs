using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Elinor
{
    class Tutorial
    {
        public static MainWindow Main;
        private static int _step;
        private static Popup _popup = new Popup();

        public static void NextTip()
        {
            if (_step >= 15) Cancel(); 
            
            _popup.IsOpen = false;
            _popup = new Popup
                         {
                             AllowsTransparency = true,
                         };

            Button btnNext = new Button
                {
                    Content = "Next",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 65,
                };
            btnNext.Click += (o, a) => NextTip();
            Button btnClose = new Button
                {
                    Content = "Close",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 65,
                };
            btnClose.Click += (o, a) => Cancel();
            StackPanel panel = new StackPanel
                                   {
                                       Margin = new Thickness(6, 6, 6, 6)
                                   };

            //panel.BorderBrush = new SolidColorBrush(Colors.Goldenrod);

            switch (_step)
            {
                case 0:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                                           {
                                               Text = "Welcome to Elinor tutorial.\n\n" +
                                                      "To proceed to the first step, please click the \"Next\" button.\n" +
                                                      "To cancel the tutorial, please click the \"Cancel\" button"
                                           });
                    _popup.PlacementTarget = Main;
                    _popup.Placement = PlacementMode.Center;
                    _popup.IsOpen = true;
                    break;

                case 1:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                                           {
                                               Text = "First you should customize a profile for your trading character."
                                           });
                    _popup.PlacementTarget = Main.cbProfiles;
                    _popup.Placement = PlacementMode.Bottom;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = 5;
                    _popup.HorizontalOffset = -50;
                    FlashControl(Main.cbProfiles, Colors.LimeGreen, Main);
                    break;

                case 2:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "You can do this either by clicking \"New...\" and entering\n" +
                               "your data manually on the \"Settings\" tab..."
                    });
                    _popup.PlacementTarget = Main.btnNew;
                    _popup.Placement = PlacementMode.Left;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = -5;
                    _popup.HorizontalOffset = -5;
                    FlashControl(Main.btnNew, Colors.LimeGreen, Main);
                    break;

                case 3:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "Or by clicking \"Import...\" and\n" +
                               "fetching your character's data from the Eve API."
                    });
                    _popup.PlacementTarget = Main.btnImport;
                    _popup.Placement = PlacementMode.Left;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = -5;
                    _popup.HorizontalOffset = -5;
                    FlashControl(Main.btnImport, Colors.LimeGreen, Main);
                    break;

                case 4:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "You can delete a profile at any time by clicking \"Delete\"."
                    });
                    _popup.PlacementTarget = Main.btnDelete;
                    _popup.Placement = PlacementMode.Left;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = -5;
                    _popup.HorizontalOffset = -5;
                    FlashControl(Main.btnDelete, Colors.LimeGreen, Main);
                    break;


                case 5:
                    SelectTab(1);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "You can set which margins should be displayed as \"good\"\n" +
                               "or \"bad\" margins on the \"Settings\" tab."
                    });
                    _popup.PlacementTarget = Main.gbMarginsSettings;
                    _popup.Placement = PlacementMode.Bottom;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = 5;
                    _popup.HorizontalOffset = 10;
                    FlashControl(Main.tiSettings, Colors.LimeGreen, Main);
                    FlashControl(Main.gbMarginsSettings, Colors.LimeGreen, Main);
                    break;

                case 6:
                    SelectTab(1);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "Here's where you can set you character's skills and standings, too."
                    });
                    _popup.PlacementTarget = Main.gbFeesAndTaxes;
                    _popup.Placement = PlacementMode.Top;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = -5;
                    _popup.HorizontalOffset = 10;
                    FlashControl(Main.gbFeesAndTaxes, Colors.LimeGreen, Main);
                    break;

                case 7:
                    SelectTab(1);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "Should you screw your settings up, you can always reset them."
                    });
                    _popup.PlacementTarget = Main.btnDefault;
                    _popup.Placement = PlacementMode.Top;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = -5;
                    _popup.HorizontalOffset = -270;
                    FlashControl(Main.btnDefault, Colors.LimeGreen, Main);
                    break;


                case 8:
                    SelectTab(0);
                    FlashControl(Main.tiOverview, Colors.LimeGreen, Main);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "Now - please open your EVE Online client.\n" +
                               "Open the market window and select an item you're interested in.\n" +
                               "Push the button named \"Export to File\" at the bottom of the market window.\n"
                    });
                    _popup.PlacementTarget = Main;
                    _popup.Placement = PlacementMode.Center;
                    _popup.IsOpen = true;
                    break;

                case 9:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "Elinor has now automatically imported the item's prices and\n" +
                               "calculated some data which can help determinate if the item\n" +
                               "is a fitting candidate for station trading."
                    });
                    _popup.PlacementTarget = Main.brdImportant;
                    _popup.Placement = PlacementMode.Bottom;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = 5;
                    FlashForeground(Main.brdImportant);
                    break;

                case 10:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "If you checked auto copy, the corresponding price plus/minus\n" +
                               ".01 ISK has now been copied to your clipboard ready to get\n" +
                               "pasted into the set up/modify order window in EVE Online."
                    });
                    _popup.PlacementTarget = Main.cbAutoCopy;
                    _popup.Placement = PlacementMode.Top;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = -5;
                    _popup.HorizontalOffset = -115;
                    FlashControl(Main.gbAutocopy, Colors.LimeGreen, Main);
                    FlashForeground(Main.cbAutoCopy);
                    break;

                case 11:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "If you don't want to work with auto copy, you can click\n" +
                               "the price tags to copy sell or buy price to your\n" +
                               "clipboard."
                    });
                    _popup.PlacementTarget = Main.sprt1;
                    _popup.Placement = PlacementMode.Bottom;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = -45;
                    _popup.HorizontalOffset = 5;
                    FlashForeground(Main.lblBuy);
                    FlashForeground(Main.lblSell);
                    break;

                case 12:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "The sample charts provide some information on Cost of Sales and\n" +
                               "Profit for some example order sizes so you can extrapolate easier if\n" +
                               "setting up an order of a given size is profitable."
                    });
                    _popup.PlacementTarget = Main.gbSamples;
                    _popup.Placement = PlacementMode.Top;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = -5;
                    _popup.HorizontalOffset = -10;
                    FlashControl(Main.dgSamples, Colors.LimeGreen, Main);
                    FlashControl(Main.dgSamplesFive, Colors.LimeGreen, Main);
                    break;

                case 13:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "By clicking on the \"Pin\" button you can make Elinor stay above the\n" +
                               "EVE Online window so you can easily modify orders in EVE while always\n" +
                               "having Elinors data within your sight."
                    });
                    _popup.PlacementTarget = Main.sbMain;
                    _popup.Placement = PlacementMode.Top;
                    _popup.IsOpen = true;
                    _popup.VerticalOffset = -5;
                    _popup.HorizontalOffset = 5;
                    FlashControl(Main.btnStayOnTop, Colors.LimeGreen, Main);
                    break;

                case 14:
                    SelectTab(0);
                    panel.Children.Add(new TextBlock
                    {
                        Text = "That's it!\n\n" +
                               "Most elements of Elinor provide a more or less helpful tooltips.\n" +
                               "Just explore a bit.\n" +
                               "\nPlease notice that Elinor is still in beta and be careful when pasting stuff\n" +
                               "into EVE Online."
                    });
                    _popup.PlacementTarget = Main;
                    _popup.Placement = PlacementMode.Center;
                    _popup.IsOpen = true;
                    FlashControl(Main.btnBeta, Colors.LimeGreen, Main);
                    FlashControl(Main.btnAbout, Colors.LimeGreen, Main);
                    FlashForeground(Main.tbStatus);

                    break;


            }
            Grid grid = new Grid();
            if (_step >= 14)
            {
                btnClose.HorizontalAlignment = HorizontalAlignment.Right;
                grid.Children.Add(btnClose);
            }
            else
            {
                grid.Children.Add(btnNext);
                grid.Children.Add(btnClose);
            }
            grid.Margin = new Thickness(0, 20, 0, 0);
            panel.Children.Add(grid);
           
            Border brd = new Border
            {
                CornerRadius = new CornerRadius(5),
                BorderBrush = new LinearGradientBrush(Colors.LightSlateGray, Colors.Black, .45),
                BorderThickness = new Thickness(1),
                Background = new LinearGradientBrush(Colors.LightYellow, Colors.PaleGoldenrod, .25),
                
               
            };
            brd.Child = panel;
            _popup.Child = brd;

            _step++;
        }

        private static void SelectTab(int i)
        {
            Main.Dispatcher.Invoke(new Action(delegate
                                                  {
                                                      Main.tabControl1.SelectedIndex = i;
                                                  }));
        }

        public static void FlashControl(Control control, Color flashColor, MainWindow main)
        {
            Thread thread = new Thread(new ThreadStart(delegate
                    {
                        Brush old = null;
                        Color oldColor;
                        main.Dispatcher.Invoke(new Action(delegate
                            {
                                old = control.BorderBrush;
                            }));
                        try
                        {
                            oldColor = ((SolidColorBrush)old).Color;
                        }
                        catch (Exception)
                        {
                            oldColor = Colors.WhiteSmoke;
                        }

                        for (int i = 0; i < 16; i++)
                        {
                            int ii = i % 4;
                            switch (ii)
                            {
                                case 0:
                                    main.Dispatcher.Invoke(new Action(delegate
                                    {
                                        control.BorderBrush = new LinearGradientBrush(flashColor, oldColor, 0);
                                    }));
                                    break;

                                case 1:
                                    main.Dispatcher.Invoke(new Action(delegate
                                    {
                                        control.BorderBrush = new SolidColorBrush(flashColor);
                                    }));
                                    break;

                                case 2:
                                    main.Dispatcher.Invoke(new Action(delegate
                                    {
                                        control.BorderBrush = new LinearGradientBrush(oldColor, flashColor, 0);
                                    }));
                                    break;

                                case 3:
                                    main.Dispatcher.Invoke(new Action(delegate
                                    {
                                        control.BorderBrush = old;
                                    }));
                                    break;

                            }
                            Thread.Sleep(175);
                        }
                        main.Dispatcher.Invoke(new Action(delegate
                        {
                            control.BorderBrush = old;
                        }));
                    }));
            thread.Start();
        }

        public static void FlashForeground(Control control)
        {
            Thread thread = new Thread(new ThreadStart(delegate
            {
                Main.Dispatcher.Invoke(new Action(delegate
                { }));

                for (int i = 0; i < 16; i++)
                {
                    int ii = i % 4;
                    switch (ii)
                    {
                        case 0:
                            Main.Dispatcher.Invoke(new Action(delegate
                            {
                                control.Opacity = .75;
                            }));
                            break;

                        case 1:
                            Main.Dispatcher.Invoke(new Action(delegate
                            {
                                control.Opacity = .5;
                            }));
                            break;

                        case 2:
                            Main.Dispatcher.Invoke(new Action(delegate
                            {
                                control.Opacity = .75;
                            }));
                            break;

                        case 3:
                            Main.Dispatcher.Invoke(new Action(delegate
                            {
                                control.Opacity = 1;
                            }));
                            break;

                    }
                    Thread.Sleep(175);
                }
                Main.Dispatcher.Invoke(new Action(delegate
                {
                    control.Opacity = 1;
                }));
            }));
            thread.Start();
        }

        public static void FlashForeground(Decorator control)
        {
            Thread thread = new Thread(new ThreadStart(delegate
                {
                    Main.Dispatcher.Invoke(new Action(delegate
                    { }));
               
                for (int i = 0; i < 16; i++)
                {
                    int ii = i % 4;
                    switch (ii)
                    {
                        case 0:
                            Main.Dispatcher.Invoke(new Action(delegate
                                                                  {
                                                                      control.Opacity = .75;
                                                                  }));
                            break;

                        case 1:
                            Main.Dispatcher.Invoke(new Action(delegate
                                                                  {
                                                                      control.Opacity = .5;
                                                                  }));
                            break;

                        case 2:
                            Main.Dispatcher.Invoke(new Action(delegate
                                                                  {
                                                                      control.Opacity = .75;
                                                                  }));
                            break;

                        case 3:
                            Main.Dispatcher.Invoke(new Action(delegate
                                                                  {
                                                                      control.Opacity = 1;
                                                                  }));
                            break;

                    }
                    Thread.Sleep(175);
                }
                Main.Dispatcher.Invoke(new Action(delegate
                {
                    control.Opacity = 1;
                }));   
            }));
            thread.Start();
        }

        public static void FlashForeground(UIElement control)
        {
            Thread thread = new Thread(new ThreadStart(delegate
            {
                Main.Dispatcher.Invoke(new Action(delegate
                { }));

                for (int i = 0; i < 16; i++)
                {
                    int ii = i % 4;
                    switch (ii)
                    {
                        case 0:
                            Main.Dispatcher.Invoke(new Action(delegate
                            {
                                control.Opacity = .75;
                            }));
                            break;

                        case 1:
                            Main.Dispatcher.Invoke(new Action(delegate
                            {
                                control.Opacity = .5;
                            }));
                            break;

                        case 2:
                            Main.Dispatcher.Invoke(new Action(delegate
                            {
                                control.Opacity = .75;
                            }));
                            break;

                        case 3:
                            Main.Dispatcher.Invoke(new Action(delegate
                            {
                                control.Opacity = 1;
                            }));
                            break;

                    }
                    Thread.Sleep(175);
                }
                Main.Dispatcher.Invoke(new Action(delegate
                {
                    control.Opacity = 1;
                }));
            }));
            thread.Start();
        }
        
        public static void Cancel()
        {
            _popup.IsOpen = false;
            _step = 0;
        }


    }
}
