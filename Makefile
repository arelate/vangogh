APP          := vangogh
VERSION      := $(shell git describe --tags --abbrev=0)
COMMIT       := $(shell git rev-parse --short HEAD)
BUILD_DATE   := `date +%FT%T%z`
LD_FLAGS     := "-s -w -X 'vangogh/version.Version=$(VERSION)' -X 'vangogh/version.Commit=$(COMMIT)' -X 'vangogh/version.BuildDate=$(BUILD_DATE)'"

vangogh:
	@ go build -ldflags=$(LD_FLAGS) -o $(APP) main.go

linux-amd64:
	@ GOOS=linux GOARCH=amd64 go build -ldflags=$(LD_FLAGS) -o $(APP)-linux-amd64 main.go

linux-arm64:
	@ GOOS=linux GOARCH=arm64 go build -ldflags=$(LD_FLAGS) -o $(APP)-linux-arm64 main.go

linux-armv7:
	@ GOOS=linux GOARCH=arm GOARM=7 go build -ldflags=$(LD_FLAGS) -o $(APP)-linux-armv7 main.go

linux-armv6:
	@ GOOS=linux GOARCH=arm GOARM=6 go build -ldflags=$(LD_FLAGS) -o $(APP)-linux-armv6 main.go

linux-armv5:
	@ GOOS=linux GOARCH=arm GOARM=5 go build -ldflags=$(LD_FLAGS) -o $(APP)-linux-armv5 main.go

darwin-amd64:
	@ GOOS=darwin GOARCH=amd64 go build -ldflags=$(LD_FLAGS) -o $(APP)-darwin-amd64 main.go

darwin-arm64:
	@ GOOS=darwin GOARCH=arm64 go build -ldflags=$(LD_FLAGS) -o $(APP)-darwin-arm64 main.go

freebsd-amd64:
	@ GOOS=freebsd GOARCH=amd64 go build -ldflags=$(LD_FLAGS) -o $(APP)-freebsd-amd64 main.go

openbsd-amd64:
	@ GOOS=openbsd GOARCH=amd64 go build -ldflags=$(LD_FLAGS) -o $(APP)-openbsd-amd64 main.go

windows-amd64:
	@ GOOS=windows GOARCH=amd64 go build -ldflags=$(LD_FLAGS) -o $(APP)-windows-amd64 main.go

build: linux-amd64 linux-arm64 linux-armv7 linux-armv6 linux-armv5 darwin-amd64 darwin-arm64 freebsd-amd64 openbsd-amd64 windows-amd64

run:
	@ go run main.go

clean:
	@ rm -f $(APP)-* $(APP)
