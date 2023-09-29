# WebAppOsloMet

Har lastet opp koden fra demo fra forelesning. Tenkte å bruke det som grunnlag. 

1: Knappen fører foreløpig til Create new Item, slik som det var i demoen, har bare flyttet litt rundt for å ha som mal

2: I Navbaren er det en lenke til SubForums, men vi får se om vi rekker å implementere dette. De andre er forslag til undersider. Har ikke laget noen enda.

3: Tenkte kanskje å ha posts i feeden nedover som en liste, slik at man kan trykke seg inn på dem. Det blir ganske likt som i demoen, men vi må bare fjerne shit vi ikke trenger som id og pris.

<img width="950" alt="image" src="https://github.com/SameNilsen/WebAppOsloMet/assets/45354242/2ba1adfa-781b-45d9-9c40-c28ba2f4a0dc">


## Update 28.09
Main feeden ser nå slik ut, så det er jo fortsatt ganske likt. Postene og brukerne ligger i databasen, selvom det ikke er så mye info på brukerne enda, tror det bare er tittel liksom. Man kan også trykke seg inn på hver enkelt post slik at man ser mer detaljer, men det er virkelig ikke noe pent design enda. På create new post siden fungerer det ganske likt som i demoen, bortsett fra litt forkjell i create funksjonen i PostController.cs siden posten skal linkes til en User før den lagres funker det ikke å bruke ModelState.IsValid funksjonen siden den krever at hele post objektet er laget klart fra frontend sida. Så vi må finne en bedre måte å validere post objektet. Men bortsett fra det funker Create som det skal tror jeg.

<img width="833" alt="image" src="https://github.com/SameNilsen/WebAppOsloMet/assets/45354242/69bf9ed8-3754-438a-a23c-5ea4fba44c72">


Har forresten også styra litt rundt med klassene og filene og mappene, slik at det litt kaos nå. Målet er å få alt bort fra demoen og de filene som hørte til der, f.eks item filene som ItemRepository og ItemController osv. Men jeg har ikke fjernet dem ettersom funksjoner jeg ikke har laget/endret på enda (som order list og sånt) fortsatt er avhengige av at de er der. De filene som derimot er en del av vårt prosjekt er: HomeController.cs, PostController.cs, DBInit.cs, IPostRepository.cs, PostRepository.cs, IUserRepository.cs, UserRepository.cs, alt i Migrations mappa, Post.cs, User.cs, PostDetailsViewModel.cs, PostListViewModel.cs, alt i Views/Home mappa og alt i Views/Post mappa, _PostsTable.cshtml og _Layout.cshtml og noen av de felles greiene. 



## Update 29.09
La til en side for visning av Posts per User. La også til litt sånne lenker mellom sidene. Hvis man trykker seg inn på en post slik at man kommer til Detailed visning av posten, kan man se hvem som posta og så trykke på navnet for å komme til en visning av alle postene denne useren har posta.
<img width="520" alt="image" src="https://github.com/SameNilsen/WebAppOsloMet/assets/45354242/51c03b2a-487c-41fe-a21e-6693aa9a3705">

Hvis man derfra trykker på tittelen på posten kommer man til detailed visning av posten, slik som det fungerer i MainFeed
<img width="835" alt="image" src="https://github.com/SameNilsen/WebAppOsloMet/assets/45354242/3ed81242-9d8a-4fa5-b123-e4eed02c5dd3">

Hvis man trykker MyPosts i NavBaren skal man i fremtiden bli sendt til en side over egne posts, men siden vi ikke har noe innlogging eller sånt enda, kommer man bare til list of posts fra første i databasen (Nå Alice Hansen)


