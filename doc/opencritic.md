# OpenCritic API

NOTE: I didn't spend any time thinking how it might be integrated in any shape or form - on the backend, frontend, etc. However, I quickly glanced at the site and inferred few current API endpoints. There doesn't seem to be a lot of documentation around - so I want to capture what I saw to potentially saw time in the future.

# Host

https://opencritic.com

# Search

* Endpoint: /api/meta/search
* Parameters: criteria string

Example: https://opencritic.com/api/meta/search?criteria=persona

Returns: [{"id":1537,"name":"Persona 5","dist":0.2,"relation":"game"},{"id":8785,"name":"Persona 5 Royal","dist":0.5,"relation":"game"},{"id":9636,"name":"Persona 4 Golden (PC)","dist":0.578947,"relation":"game"},{"id":982,"name":"Chris Person","dist":0.6,"relation":"critic"},{"id":8709,"name":"One Person Story","dist":0.684211,"relation":"game"},{"id":4184,"name":"Joel Peterson","dist":0.705882,"relation":"critic"},{"id":6898,"name":"Cody Peterson","dist":0.705882,"relation":"critic"},{"id":345,"name":"Erik Pederson","dist":0.705882,"relation":"critic"},{"id":2408,"name":"Persona 4: Dancing All Night","dist":0.714286,"relation":"game"},{"id":470,"name":"Blake Peterson","dist":0.722222,"relation":"critic"}]

# Game 

* Endpoint: /api/game/{id}
* Parameters: id int

    Example: https://opencritic.com/api/game/1537

 Returns: {"reviewSummary":{"completed":false},"mastheadScreenshot":{"fullRes":"//c.opencritic.com/images/games/1537/i0rCFDNBcpzxV1NgPN.jpg","thumbnail":"//c.opencritic.com/images/games/1537/i0rCFDNBcpzxV1NgPN_th.jpg"},"bannerScreenshot":{"fullRes":"//c.opencritic.com/images/games/1537/banner.jpg"},"Rating":{"imageSrc":"http://www.esrb.org/images/ratingsymbol_m.png"},"newsSearchEnabled":true,"type":"BASE","Skus":["NZ_1783276b-acec-41f6-bc8a-1e0d999660ec"],"percentRecommended":99.1869918699187,"numReviews":126,"numTopCriticReviews":92,"medianScore":95,"averageScore":94.47933884297521,"topCriticScore":93.82608695652173,"percentile":100,"tier":"Mighty","hasLootBoxes":false,"isMajorTitle":false,"name":"Persona 5","description":"Persona 5 is a game about the internal and external conflicts of a group of troubled high school students - the protagonist and a collection of compatriots he meets in the game's story - who live dual lives as Phantom Thieves.\n\nPersona 5 is a game about the internal and external conflicts of a group of troubled high school students - the protagonist and a collection of compatriots he meets in the game's story - who live dual lives as Phantom Thieves. They have the typically ordinary day-to-day of a Tokyo high schooler: attending class, after-school activities, and part-time jobs. But they also undertake fantastical adventures by using otherworldly powers to enter the hearts of people. Their power comes from the Persona, the Jungian concept of the \"self\"; the game's heroes realize that society forces people to wear masks to protect their inner vulnerabilities, and by literally ripping off their protective mask and confronting their inner selves do the heroes awaken their inner power, and using it to help those in need. Ultimately, the group of Phantom Thieves seeks to change their day-to-day world to match their perception and see through the masks modern society wears.","screenshots":[{"fullRes":"//c.opencritic.com/images/games/1537/12W0eZPuItxCt6byCafUezMoUIHBNkIA.jpg","thumbnail":"//c.opencritic.com/images/games/1537/12W0eZPuItxCt6byCafUezMoUIHBNkIA_th.jpg"},{"fullRes":"//c.opencritic.com/images/games/1537/OtTUDHGFh0X3QkKBCmeSE9bm7vX9u9wg.jpg","thumbnail":"//c.opencritic.com/images/games/1537/OtTUDHGFh0X3QkKBCmeSE9bm7vX9u9wg_th.jpg"},{"fullRes":"//c.opencritic.com/images/games/1537/Y4PmQpABUQTUdTTTigiDJJruOSUH3mI8.jpg","thumbnail":"//c.opencritic.com/images/games/1537/Y4PmQpABUQTUdTTTigiDJJruOSUH3mI8_th.jpg"}],"trailers":[{"publishedDate":"2015-02-05T00:00:00.000Z","title":"Persona 5 Gameplay Trailer","videoId":"wPqSkzNNPIg","externalUrl":"https://www.youtube.com/watch?v=wPqSkzNNPIg","channelTitle":"IGN","channelId":"UCKy1dAqELo0zrOtPkf0eTMw","description":"See the first gameplay and a lots of stylish story action in this Japanese Persona 5 trailer."}],"twitchName":"Persona 5","embargoDate":"2017-03-29T16:03:00.000Z","steamId":null,"Companies":[{"name":"Atlus","type":"PUBLISHER"}],"Platforms":[{"id":6,"name":"PlayStation 4","shortName":"PS4","imageSrc":"https://c.opencritic.com/images/platforms/ps4.png","releaseDate":"2017-04-04T00:00:00.000Z","displayRelease":null}],"Genres":[{"id":77,"name":"RPG"}],"Affiliates":[],"id":1537,"firstReleaseDate":"2017-04-04T00:00:00.000Z","createdAt":"2019-09-21T18:54:24.697Z","updatedAt":"2020-03-09T19:38:28.035Z","firstReviewDate":"2017-03-06T05:00:00.000Z","latestReviewDate":"2019-06-04T04:00:00.000Z"} 