﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using Melville.MVVM.FileSystem;
using Melville.MVVM.RunShellCommands;
using Melville.MVVM.Wpf.RootWindows;
using Moq;
using WebDashboard.Views;
using Xunit;

namespace WebDashboard.Test.Views
{
    public class DeploymentViewModelTest
    {
        private readonly Mock<INavigationWindow> nav = new();
        private readonly Mock<IRootViewModel> rvm = new();
        private readonly Mock<IDirectory> projectDir = new();
        private readonly Mock<IFile> projectFile = new();
        private readonly Mock<IFile> configFile = new();
        private readonly Mock<IFile> publishFile = new();
        private readonly MemoryStream configStream = new();
        private readonly Mock<IRunShellCommand> runner = new();
        private readonly Mock<IProcessProxy> proxy = new();
        private readonly Mock<IHasPassword> password = new();
        
        
        private readonly DeploymentViewModel sut;

        public DeploymentViewModelTest()
        {
            rvm.Setup(i => i.ProjectFile()).Returns(projectFile.Object);
            projectFile.SetupGet(i => i.Directory).Returns(projectDir.Object);
            projectFile.SetupGet(i => i.Path).Returns(@"C:\dir\Target.csproj");
            projectDir.Setup(i => i.File("web.config")).Returns(configFile.Object);
            publishFile.SetupGet(i => i.Name).Returns("PubFile.pubxml");
            configFile.Setup(i => i.CreateWrite(FileAttributes.Normal)).ReturnsAsync(configStream);
            proxy.SetupGet(i => i.HasExited).Returns(true);
            rvm.SetupGet(i => i.WebConfig).Returns("New Web Config");
            rvm.Setup(i => i.PublishFile()).Returns(publishFile.Object);
            runner.Setup(i => i.CapturedShellExecute("dotnet", It.IsAny<string>())).Returns(proxy.Object);
            sut = new DeploymentViewModel(nav.Object, rvm.Object, password.Object);
        }

        [Fact]
        public void NavigateBack()
        {
            sut.Return();
            nav.Verify(i=>i.NavigateTo(rvm.Object), Times.Once);
            nav.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RunDeployment()
        {
            password.Setup(i => i.Password()).Returns("StupidPass");
            await sut.RunDeployment(runner.Object);
            Assert.Equal("New Web Config", Encoding.UTF8.GetString(configStream.ToArray()));
            runner.Verify(i=>i.CapturedShellExecute("dotnet", 
                @"publish C:\dir\Target.csproj /p:PublishProfile=PubFile /p:Password=StupidPass"), Times.Once);
            configFile.Verify(i=>i.Delete(), Times.Once);
        }
    }
}