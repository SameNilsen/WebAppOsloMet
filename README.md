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

