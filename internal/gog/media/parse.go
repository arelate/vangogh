package media

import (
	"errors"
	"flag"
)

func Parse(mt string) Type {
	switch mt {
	// game
	case gameAbbr:
		fallthrough
	case game:
		fallthrough
	case games:
		return Game
	// movie
	case movieAbbr:
		fallthrough
	case movie:
		fallthrough
	case movies:
		return Movie
	// unknown
	default:
		return Unknown
	}
}

func ParseArgs(cmd string, args []string) (Type, error) {

	var mediaFlag string
	mt := Game

	fetchFlags := flag.NewFlagSet(cmd, flag.ExitOnError)
	fetchFlags.StringVar(&mediaFlag, Flag, "", FlagDesc)
	fetchFlags.StringVar(&mediaFlag, FlagAlias, "", FlagDesc)

	err := fetchFlags.Parse(args)
	if err != nil {
		return Unknown, err
	}

	if mediaFlag != "" {
		mt = Parse(mediaFlag)
		if mt == Unknown {
			return Unknown, errors.New("unknown media type: " + mediaFlag)
		}
	}

	return mt, nil
}
