package fetchall

import (
	"context"
	"errors"
	"flag"
	"fmt"
	"github.com/boggydigital/vangogh/cmd/help"
	"github.com/boggydigital/vangogh/internal/gog/local"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/remote"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

func printDot(index int) {
	fmt.Print(".")
}

func Run(httpClient *http.Client, mongoClient *mongo.Client, ctx context.Context, args []string) error {

	if len(args) < 1 {
		args = make([]string, 1)
		args[0] = Cmd
		return help.Run(args)
	}

	var mediaFlag string
	mt := media.Game

	fetchFlags := flag.NewFlagSet(Cmd, flag.ExitOnError)
	fetchFlags.StringVar(&mediaFlag, MediaFlag, "", MediaFlagDesc)
	fetchFlags.StringVar(&mediaFlag, MediaFlagAlias, "", MediaFlagDesc)

	err := fetchFlags.Parse(args[1:])
	if err != nil {
		return err
	}

	if mediaFlag != "" {
		mt = media.Parse(mediaFlag)
		if mt == media.Unknown {
			return errors.New("unknown media type: " + mediaFlag)
		}
	}

	var pageTransferor remote.PageTransferor
	var setter local.Setter

	switch args[0] {
	case ProductsAlias:
		args[0] = Products
		fallthrough
	case Products:
		pageTransferor = remote.NewProductPage(httpClient, mt)
		setter = local.NewProducts(mongoClient, ctx)
	case AccountProductsAlias:
		args[0] = AccountProducts
		fallthrough
	case AccountProducts:
		pageTransferor = remote.NewAccountProductPage(httpClient, mt)
		setter = local.NewAccountProducts(mongoClient, ctx)
	case WishlistAlias:
		args[0] = Wishlist
		fallthrough
	case Wishlist:
		pageTransferor = remote.NewWishlistPage(httpClient, mt)
		setter = local.NewWishlist(mongoClient, ctx)
	default:
		return errors.New(fmt.Sprintf("unknown fetch source: %s", args[0]))
	}

	if pageTransferor == nil ||
		setter == nil {
		return errors.New("error creating source or destination for " + Cmd)
	}

	totalPages := 1
	for page := 1; page <= totalPages; page++ {
		fmt.Println(Cmd, args[0], fmt.Sprintf("(%s)", mt.String()), page, "/", totalPages)
		totalPages, err = pageTransferor.TransferPage(page, setter, printDot)
		fmt.Println()
		if err != nil {
			return err
		}
	}

	return nil
}
