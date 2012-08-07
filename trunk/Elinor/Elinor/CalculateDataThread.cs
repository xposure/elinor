using System;
using System.Data;
using System.Windows.Media;


namespace Elinor
{
    class CalculateDataThread
    {
        private double _sellPrice, _buyPrice;
        private MainWindow _main;

        public CalculateDataThread(double sell, double buy,MainWindow main)
        {
            _sellPrice = sell;
            _buyPrice = buy;
            _main = main;
        }

        internal static double BrokerFee(int brokerRelations, double corpStanding, double factionStanding)
        {
            //BrokerFee % = (1.000 % – 0.050 % × BrokerRelationsSkillLevel) / 2 ^ (-0.14 × FactionStanding - 0.06 × CorporationStanding)
            return ((.01 - (brokerRelations * .0005)) / Math.Pow(2, ((0.14 * factionStanding) + (.06 * corpStanding))));
        }

        internal static double SalesTax(int accounting)
        {
            //return .015 - .015 * (accounting * .1);
            return .015*(1 - (accounting*.1));
        }

        internal void Run()
        {
            double brokerFee = BrokerFee(_main.Settings.BrokerRelations, _main.Settings.CorpStanding, _main.Settings.FactionStanding);
            double salesTax = SalesTax(_main.Settings.Accounting);
            
            
            double revenue = ((_sellPrice - .01) - (_sellPrice - .01) * brokerFee - (_sellPrice - .01) * salesTax);
            double cos = (_buyPrice + .01) + (_buyPrice + .01)*brokerFee;

            //double tenPercent = -(((.1*revenue) - revenue)/(1 + brokerFee));

            DataTable samples = new DataTable();
            samples.Columns.Add("Size", typeof (string));
            samples.Columns.Add("CoS", typeof(string));
            samples.Columns.Add("Profit", typeof (string));
            DataTable samplesF = new DataTable();
            samplesF.Columns.Add("Size", typeof(string));
            samplesF.Columns.Add("CoS", typeof(string));
            samplesF.Columns.Add("Profit", typeof(string));
            
            for (int i = 1; i < 100000000;i *= 10 )
            {
                samples.Rows.Add(String.Format("{0:n0}", i), String.Format("{0:n}m", i * cos / 1000000), String.Format("{0:n}m", i * (revenue - cos) / 1000000));
                samplesF.Rows.Add(String.Format("{0:n0}", i * 5), String.Format("{0:n}m", i * cos * 5 / 1000000), String.Format("{0:n}m", i * 5 * (revenue - cos) / 1000000));
            }
                _main.Dispatcher.Invoke(new Action(delegate
                                                       {
                                                           _main.lblRevenue.Content = String.Format("{0:n} ISK", revenue);
                                                           _main.lblCoS.Content = String.Format("{0:n} ISK", cos);
                                                           _main.lblProfit.Content = String.Format("{0:n} ISK", revenue - cos);
                                                           double margin = 100 * (revenue - cos) / revenue;
                                                           _main.lblMargin.Content = String.Format("{0:n}%", margin);
                                                           //_main.lbl10pat.Content = String.Format("{0:n} ISK", tenPercent);
                                                           double markup = 100*(revenue - cos)/cos;
                                                           _main.lblMarkup.Content = Math.Abs(markup) < 10000 ? 
                                                               String.Format("{0:n}%", markup) :
                                                               (markup > 0 ? "∞%" : "-∞%");

                                                           if (margin/100 >= _main.Settings.MarginThreshold)
                                                           {
                                                               _main.lblMargin.Foreground = new SolidColorBrush(Colors.ForestGreen);
                                                               _main.brdImportant.BorderBrush = new LinearGradientBrush(Colors.LimeGreen, Colors.ForestGreen, 45.0);
                                                           }
                                                           else if (margin/100 > _main.Settings.MinimumThreshold)
                                                           {
                                                               _main.lblMargin.Foreground = new SolidColorBrush(Colors.Orange);
                                                               _main.brdImportant.BorderBrush = new LinearGradientBrush(Colors.Yellow, Colors.Orange, 45.0);
                                                           }
                                                           else
                                                           {
                                                               _main.lblMargin.Foreground = new SolidColorBrush(Colors.Red);
                                                               _main.brdImportant.BorderBrush = new LinearGradientBrush(Colors.OrangeRed, Colors.Red, 45.0);
                                                           }
                                                           
                                                           _main.dgSamples.ItemsSource = samples.AsDataView();
                                                           _main.dgSamplesFive.ItemsSource = samplesF.AsDataView();
                                                           
                                                           
                                                          }));
        }
    }
}
