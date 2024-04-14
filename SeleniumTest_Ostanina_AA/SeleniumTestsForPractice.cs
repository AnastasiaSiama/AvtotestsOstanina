using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTest_Ostanina_AA;

public class SeleniumTestsForPractice
{
    [Test]
    public void Authorization()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        //зайти в хром (с помощью драйвера)
        var driver = new ChromeDriver(options);

        //перейти по урлу https://staff-testing.testkontur.ru
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        Thread.Sleep(5000);

        // ввести логин и пароль
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("Siama16@mail.ru");
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("Tinctura92!");
        Thread.Sleep(5000);

        // нажать на кнопку "войти" 
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        Thread.Sleep(5000);

        // проверяем что пользователь на нужной странице
        var currentUrl = driver.Url;
        Assert.That(currentUrl == "https://staff-testing.testkontur.ru/news");

        // закрываем браузер и убиваем процесс драйвера
        driver.Quit();
    }
}