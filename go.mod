module github.com/boggydigital/vangogh

go 1.15

require (
	github.com/arelate/gog_auth v0.1.3-alpha
	github.com/arelate/gog_types v0.1.6-alpha
	github.com/arelate/gog_urls v0.1.7-alpha
	github.com/arelate/vangogh_types v0.1.0-alpha
	github.com/arelate/vangogh_urls v0.1.0-alpha
	github.com/boggydigital/clo v1.0.3
	github.com/boggydigital/dolo v0.1.4-alpha
	github.com/boggydigital/kvas v0.1.9-alpha
)

replace (
	github.com/arelate/gog_auth => ../gog_auth
	github.com/arelate/gog_types => ../gog_types
	github.com/arelate/gog_urls => ../gog_urls
	github.com/arelate/vangogh_types => ../vangogh_types
	github.com/arelate/vangogh_urls => ../vangogh_urls
	github.com/boggydigital/clo => ../clo
	github.com/boggydigital/dolo => ../dolo
	github.com/boggydigital/kvas => ../kvas
)
