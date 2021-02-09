module github.com/boggydigital/vangogh

go 1.15

require (
	github.com/arelate/gogauth v0.1.2-alpha
	github.com/arelate/gogtypes v0.1.5-alpha
	github.com/arelate/gogurls v0.1.3-alpha
	github.com/boggydigital/clo v1.0.3
	github.com/boggydigital/kvas v0.1.5-alpha
)

replace (
	github.com/arelate/gogauth => ../gogauth
	github.com/arelate/gogtypes => ../gogtypes
	github.com/arelate/gogurls => ../gogurls
	github.com/boggydigital/clo => ../clo
	github.com/boggydigital/kvas => ../kvas
)
