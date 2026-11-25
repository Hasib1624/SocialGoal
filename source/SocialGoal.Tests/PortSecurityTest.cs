using System.Net;
using NUnit.Framework;
using SocialGoal.Mappings;
using SocialGoal.Model.Models;
using SocialGoal.Web.Services;
using SocialGoal.Web.ViewModels;

namespace SocialGoal.Tests
{
    public class PortSecurityTest
    {
        [TestCase("127.0.0.1")]
        [TestCase("10.0.0.1")]
        [TestCase("172.16.0.1")]
        [TestCase("192.168.1.1")]
        [TestCase("169.254.169.254")]
        [TestCase("100.64.0.1")]
        [TestCase("0.0.0.0")]
        [TestCase("::1")]
        [TestCase("fe80::1")]
        [TestCase("fd00::1")]
        [TestCase("::ffff:127.0.0.1")]
        [TestCase("2002:7f00:1::")]
        public void Image_download_refuses_non_public_address(string address) =>
            Assert.That(ImageFetcher.IsPublic(IPAddress.Parse(address)), Is.False);

        [TestCase("93.184.216.34")]
        [TestCase("2606:4700::1111")]
        public void Image_download_allows_public_address(string address) =>
            Assert.That(ImageFetcher.IsPublic(IPAddress.Parse(address)), Is.True);

        [TestCase("<script>alert(1)</script>", true)]
        [TestCase("<img src=x onerror=alert(1)>", true)]
        [TestCase("&#60;b", true)]
        [TestCase("1 < 2 & 3 <= 4", false)]
        public void Markup_in_input_is_detected(string value, bool dangerous) =>
            Assert.That(RequestValidation.IsDangerous(value), Is.EqualTo(dangerous));

        [Test]
        public void Mapping_a_missing_record_gives_null() =>
            Assert.That(Mapper.Map<Goal, GoalFormModel>(null), Is.Null);

        [Test]
        public void Cropper_refuses_data_that_is_not_an_image() =>
            Assert.That(ImageCropper.CropToPng(new byte[] { 1, 2, 3 }, 0, 0, 8, 8), Is.Null);
    }
}
