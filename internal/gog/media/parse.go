// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

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

func ParseArgs(cmd string, args []string) (Type, []string, error) {

	var mediaFlag string
	mt := Game

	flagSet := flag.NewFlagSet(cmd, flag.ExitOnError)
	flagSet.StringVar(&mediaFlag, Flag, "", FlagDesc)
	flagSet.StringVar(&mediaFlag, FlagAlias, "", FlagDesc)

	err := flagSet.Parse(args)
	if err != nil {
		return Unknown, nil, err
	}

	if mediaFlag != "" {
		mt = Parse(mediaFlag)
		if mt == Unknown {
			return Unknown, flagSet.Args(), errors.New("unknown media type: " + mediaFlag)
		}
	}

	return mt, flagSet.Args(), nil
}
