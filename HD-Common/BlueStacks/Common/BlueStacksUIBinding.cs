using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlueStacks.Common
{
	[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
	public class BlueStacksUIBinding : INotifyPropertyChanged
	{
		public static BlueStacksUIBinding Instance
		{
			get
			{
				if (BlueStacksUIBinding._Instance == null)
				{
					BlueStacksUIBinding._Instance = new BlueStacksUIBinding();
				}
				return BlueStacksUIBinding._Instance;
			}
			set
			{
				BlueStacksUIBinding._Instance = value;
			}
		}

		private BlueStacksUIBinding()
		{
			LocaleStrings.SourceUpdatedEvent += this.Locale_Updated;
			BluestacksUIColor.SourceUpdatedEvent += this.BluestacksUIColor_Updated;
			CustomPictureBox.SourceUpdatedEvent += this.BluestacksImage_Updated;
		}

		public void Locale_Updated(object sender, EventArgs e)
		{
			this.NotifyPropertyChanged("LocaleModel");
		}

		public void BluestacksUIColor_Updated(object sender, EventArgs e)
		{
			this.NotifyPropertyChanged("ColorModel");
			this.NotifyPropertyChanged("GeometryModel");
			this.NotifyPropertyChanged("CornerRadiusModel");
			this.NotifyPropertyChanged("TransformModel");
		}

		public void BluestacksImage_Updated(object sender, EventArgs e)
		{
			this.NotifyPropertyChanged("ImageModel");
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public Dictionary<string, Brush> ColorModel
		{
			get
			{
				return BlueStacksUIColorManager.AppliedTheme.DictBrush;
			}
			set
			{
			}
		}

		public Dictionary<string, Geometry> GeometryModel
		{
			get
			{
				return BlueStacksUIColorManager.AppliedTheme.DictGeometry;
			}
			set
			{
			}
		}

		public Dictionary<string, CornerRadius> CornerRadiusModel
		{
			get
			{
				return BlueStacksUIColorManager.AppliedTheme.DictCornerRadius;
			}
			set
			{
			}
		}

		public Dictionary<string, Transform> TransformModel
		{
			get
			{
				return BlueStacksUIColorManager.AppliedTheme.DictTransform;
			}
			set
			{
			}
		}

		public Dictionary<string, string> LocaleModel
		{
			get
			{
				return LocaleStrings.DictLocalizedString;
			}
			set
			{
			}
		}

		public Dictionary<string, Tuple<BitmapImage, bool>> ImageModel
		{
			get
			{
				return CustomPictureBox.sImageAssetsDict;
			}
			set
			{
			}
		}

		public void NotifyPropertyChanged(string name)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(name));
		}

		public static void Bind(UserControl uc, string path)
		{
			BindingOperations.SetBinding(uc, FrameworkElement.ToolTipProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(CustomRadioButton tb, string path)
		{
			BindingOperations.SetBinding(tb, ContentControl.ContentProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(ToggleButton tb, string path)
		{
			BindingOperations.SetBinding(tb, ContentControl.ContentProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(GroupBox gb, string path)
		{
			BindingOperations.SetBinding(gb, HeaderedContentControl.HeaderProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(Label label, string path)
		{
			BindingOperations.SetBinding(label, ContentControl.ContentProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(Run run, string path)
		{
			BindingOperations.SetBinding(run, TextBlock.TextProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(Window wind, string path)
		{
			BindingOperations.SetBinding(wind, Window.TitleProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(TextBlock tb, string path, string stringFormat = "")
		{
			Binding localeBinding = BlueStacksUIBinding.GetLocaleBinding(path);
			if (!string.IsNullOrEmpty(stringFormat))
			{
				localeBinding.StringFormat = stringFormat;
			}
			BindingOperations.SetBinding(tb, TextBlock.TextProperty, localeBinding);
		}

		public static void Bind(DependencyObject icon, string path, DependencyProperty dp)
		{
			BindingOperations.SetBinding(icon, dp, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void ClearBind(DependencyObject icon, DependencyProperty dp)
		{
			BindingOperations.ClearBinding(icon, dp);
		}

		public static void Bind(ComboBoxItem comboBoxItem, string path)
		{
			BindingOperations.SetBinding(comboBoxItem, ContentControl.ContentProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(Button button, string path)
		{
			BindingOperations.SetBinding(button, ContentControl.ContentProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(Image button, string path)
		{
			BindingOperations.SetBinding(button, FrameworkElement.ToolTipProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static void Bind(TextBox textBox, string path)
		{
			BindingOperations.SetBinding(textBox, TextBox.TextProperty, BlueStacksUIBinding.GetLocaleBinding(path));
		}

		public static Binding GetLocaleBinding(string path)
		{
			Binding binding = new Binding
			{
				Source = BlueStacksUIBinding.Instance
			};
			string text = "";
			if (path != null)
			{
				for (int i = 0; i < path.Length; i++)
				{
					text = text + "^" + path[i].ToString();
				}
			}
			binding.Path = new PropertyPath("Instance.LocaleModel.[" + text.ToUpper(CultureInfo.InvariantCulture) + "]", new object[0]);
			binding.Mode = BindingMode.OneWay;
			binding.FallbackValue = LocaleStrings.RemoveConstants(path);
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			LocaleStrings.AppendLocaleIfDoesntExist(path, LocaleStrings.RemoveConstants(path));
			return binding;
		}

		public static void BindColor(DependencyObject dObj, DependencyProperty dp, string path)
		{
			BindingOperations.SetBinding(dObj, dp, BlueStacksUIBinding.GetColorBinding(path));
		}

		public static void BindCornerRadius(DependencyObject dObj, DependencyProperty dp, string path)
		{
			BindingOperations.SetBinding(dObj, dp, BlueStacksUIBinding.GetCornerRadiusBinding(path));
		}

		public static void BindCornerRadiusToDouble(DependencyObject dObj, DependencyProperty dp, string path)
		{
			BindingOperations.SetBinding(dObj, dp, BlueStacksUIBinding.GetCornerRadiusDoubleBinding(path));
		}

		public static Binding GetColorBinding(string path)
		{
			return new Binding
			{
				Converter = new BrushToColorConvertor(),
				Source = BlueStacksUIBinding.Instance,
				Path = new PropertyPath("Instance.ColorModel.[" + path + "]", new object[0]),
				Mode = BindingMode.OneWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
		}

		public static Binding GetCornerRadiusBinding(string path)
		{
			return new Binding
			{
				Converter = new CornerRadiusToThicknessConvertor(),
				Source = BlueStacksUIBinding.Instance,
				Path = new PropertyPath("Instance.CornerRadiusModel.[" + path + "]", new object[0]),
				Mode = BindingMode.OneWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
		}

		public static Binding GetCornerRadiusDoubleBinding(string path)
		{
			return new Binding
			{
				Converter = new CornerRadiusToDoubleConvertor(),
				Source = BlueStacksUIBinding.Instance,
				Path = new PropertyPath("Instance.CornerRadiusModel.[" + path + "]", new object[0]),
				Mode = BindingMode.OneWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
		}

		public static void Bind(Image button, DependencyProperty dp, string path)
		{
			BindingOperations.SetBinding(button, dp, BlueStacksUIBinding.GetImageBinding(path));
		}

		public static Binding GetImageBinding(string path)
		{
			return new Binding
			{
				Source = BlueStacksUIBinding.Instance,
				Path = new PropertyPath("Instance.ImageModel.[" + path + "].Item1", new object[0]),
				Mode = BindingMode.OneWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
		}

		public static void BindTransform(DependencyObject button, DependencyProperty dp, string path)
		{
			BindingOperations.SetBinding(button, dp, BlueStacksUIBinding.GetTransformBinding(path));
		}

		public static Binding GetTransformBinding(string path)
		{
			return new Binding
			{
				Source = BlueStacksUIBinding.Instance,
				Path = new PropertyPath("Instance.TransformModel.[" + path + "]", new object[0]),
				Mode = BindingMode.OneWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
		}

		private static BlueStacksUIBinding _Instance;
	}
}


