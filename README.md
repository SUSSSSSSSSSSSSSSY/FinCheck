# FinCheck и Презентация

https://itstepedu-my.sharepoint.com/:p:/g/personal/shvec_ct78_student_itstep_org/EReCrlpsh_pPr1ls4BP-deEBUzuX_m_zEUzdigUsrjqykg?e=Uvo7lt


приложение FinCheck для учета персональных финансов. Приложение должно поддерживать возможности:

несколько валют ❌
несколько кошельков ❌
добавление/удаление источников дохода; ❌
добавление/удаление категорий расходов; ❌
учет расходов и доходов; ❌
просмотр доходов и расходов за определенный период времени в виде
графиков; ❌
перевод средств в другие валюты ❌
перевод средств на другие кошельки ❌
интерфейс ❌
база данных ❌

Программа должна сохранять отчёты о финансах, может быть много пользователей, но для каждого из них нужен пароль и логин, можно реализовать шифрование. У каждого пользователя может быть сколько угодно кошельков, нужно реализовать перевод денег между кошельками любых пользователей. Нужно реализовать перевод валют внутри кошелька из одной валюты в другую(курс валют надо получать через .api). Доходы/расходы должны быть 2х вариантов: 1-регулярные(каждый месяц повторяються), 2 - единичные(выполняются только один раз). Программа должна иметь возможность следить за системным временем. К каждому доходу/расходу должно дописываться время создания, а также описание, которое прописывается пользователем. Также следует добавить учёт статистики за месяц, квартал, год в зависимости от доходов/расходов. Пользователь должен иметь возможность удаления/добавления/редактирования в своём кошельке всех доходов/расходов. У всех расходов должна быть одна из этих, задаваемых пользователем, критерий:
Еда
Развлечения
Медикаменты
Транспорт
Коммунальные услуги
Другое
База данных должна быть SQLite
Программа должна быть написана на C#
Интерфейс должен быть реализован в WPF( желательно многооконный )
