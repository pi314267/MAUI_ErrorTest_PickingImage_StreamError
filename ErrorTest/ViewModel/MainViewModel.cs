using ErrorTest.Helper;
using ErrorTest.Model;
using NativeMedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ErrorTest.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public int SelectionLimit { get; set; } = 10;
        public bool UseCreateChooser { get; set; } = true;

        private IEnumerable<PickImages> selectedItems;
        public IEnumerable<PickImages> SelectedItems
        {
            get
            {
                return selectedItems;
            }
            set
            {
                SetProperty(ref selectedItems, value);
            }
        }
        public ICommand ImageTapGestureCommand { protected set; get; }
        public ICommand BoxViewTapCommand { protected set; get; }
        public ICommand PickImageCommand { protected set; get; }
        public MainViewModel()
        {
            PickImageCommand = new Microsoft.Maui.Controls.Command<View>(view => Pick(view, MediaFileType.Image));
            BoxViewTapCommand = new Microsoft.Maui.Controls.Command(BoxViewTapClicked);
            ImageTapGestureCommand = new Microsoft.Maui.Controls.Command(ImageTapClicked);
        }

        void Pick(View view, params MediaFileType[] types)
        {
            try
            {
                Task.Run(async () =>
                {
                    CancellationTokenSource cts = null;

                    try
                    {
                        //DisposeItems();

                        cts = new CancellationTokenSource(
                            TimeSpan.FromMilliseconds(5000000));

                        var result = await MediaGallery.PickAsync(
                            new MediaPickRequest(SelectionLimit, types)
                            {
                                Title = $"Select {SelectionLimit} photos",
                                PresentationSourceBounds = view?.GetAbsoluteBounds(40),
                                UseCreateChooser = UseCreateChooser
                            },
                            cts.Token);


                        //SelectedItems = result?.Files;

                        List<Image> liImageSource = new List<Image>();

                        //기존의 
                        List<PickImages> list = new List<PickImages>();
                        if (SelectedItems != null)
                            list = SelectedItems.ToList();//기존의 이미지들을 그대로 가져 온다

                        foreach (var item in await SetImage(result))
                        {

                            if (list.Count > 10)
                            {
                                //10개 이상의 이미지가 있을 수 없으므로 그냥 Retrun 함
                                //추후 display alert 필요
                                //"알림 : 이미지는 10개 이상 등록 불가 합니다"
                                return;
                            }

                            //ThumNail 이미지 생성
                            Image image = ImageHelper.ImageThumnailMaker(item);
                            //liImageSource.Add(image);

                            //메인이미지 존재 여부 확인
                            bool blcheckMainimg = false;
                            foreach (var s_item in list)
                            {
                                if (s_item.MainImageYN.Equals("Y"))
                                {
                                    blcheckMainimg = true;
                                    break;
                                }
                            }
                            //메인 이미지가 있다면 나머지는 모두 Main 이미지 없다고 Marking 필요
                            if (blcheckMainimg)
                            {
                                list.Add(new PickImages { CombineDeleteMarkImage = image.Source, MainImageYN = "N" });
                            }
                            else//메인 이미지가 없다면 가장 최초의 이미지가 메인 이미지가 되게 하고 이후는 Loop를 돌면서 "N"로 Setting 하게 함
                            {
                                //메인이미지는 또 다른 이미지를 Combine 시킨다
                                //Image mImage = ImageHelper.CombineWithMain(item);
                                list.Add(new PickImages { CombineDeleteMarkImage = image.Source, MainImageYN = "Y" });
                            }

                        }

                        //SelectedItems = null;
                        //DisposeItems();

                        SelectedItems = list;

                        //SetInfo(SelectedItems);

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        cts?.Dispose();

                    }
                });
            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// pick 한 이미지를 새로운 경로로 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<IEnumerable<string>> SetImage(MediaPickResult result)
        {

            IMediaFile[] files = null;
            List<string> newfile_list = new List<string>();

            files = result?.Files?.ToArray();

            foreach (var file in files)
            {
                var fileName = file.NameWithoutExtension;
                var extention = file.Extension;

                var newFile = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, fileName + "." + extention);
                using (var stream = await file.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);
                newfile_list.Add(newFile);

            }

            return newfile_list.ToArray();

        }

        private void BoxViewTapClicked(object obj)
        {

        }

        private void ImageTapClicked(object obj)
        {

        }
    }
}
