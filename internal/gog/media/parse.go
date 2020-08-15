package media

import "errors"

func Parse(t string) (Type, error) {
	if t == "game" {
		return Game, nil
	} else if t == "movie" {
		return Movie, nil
	} else {
		return Unknown, errors.New("unknown media type")
	}
}
