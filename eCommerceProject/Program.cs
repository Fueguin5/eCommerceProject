using System;
using System.Collections.Generic;
using eCommercePlatform.Models;
using Library.eCommerce.Services;

namespace ECommerce
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Product?> list = ProductServiceProxy.Current.Products;

            List<Product?> shoppingCart = ShoppingCartServiceProxy.Current.Products;

            char invOrShop = 'Z';
            do
            {
                Console.WriteLine("Welcome to Congo!");
                Console.WriteLine("I. Inventory");
                Console.WriteLine("S. Shopping Cart");
                Console.WriteLine("Q. Quit");

                string? invOrShopInput = Console.ReadLine();
                if (invOrShopInput != null)
                {
                    invOrShop = invOrShopInput[0];
                }

                if (invOrShop == 'I' || invOrShop == 'i')
                {
                    ManageInventory(list);
                }
                else if (invOrShop == 'S' || invOrShop == 's')
                {
                    ManageShoppingCart(list, shoppingCart);
                }
                else if (invOrShop != 'Q' && invOrShop != 'q')
                {
                    Console.WriteLine("Invalid input. Try again.");
                }
            } while (invOrShop != 'Q' && invOrShop != 'q');

            Checkout(shoppingCart);
        }


        static internal void Checkout(List<Product?> shoppingCart)
        {
            Console.WriteLine("\n--- Reciept ---");

            if (shoppingCart.Count == 0)
            {
                Console.WriteLine("Your shopping cart is empty, thank you for shopping at Congo!");
            }
            else
            {
                double? subTotal= 0;
                decimal salesTax = 0;
                decimal total = 0;
                shoppingCart.ForEach(Console.WriteLine);
                foreach (var product in shoppingCart)
                {
                    if (product != null && product.Price != null)
                    {
                        subTotal += product.Price * product.Quantity;
                    }
                }

                decimal convertedSubTotal = (decimal)(subTotal ?? 0.00);
                convertedSubTotal = Math.Round(convertedSubTotal, 2);
                salesTax = convertedSubTotal * 0.07m;
                total = convertedSubTotal + salesTax;

                Console.WriteLine($"Subtotal: ${subTotal}");
                Console.WriteLine($"Sales Tax (7%): ${salesTax}");
                Console.WriteLine($"Total: ${total}");
                Console.WriteLine("Thank you for shopping at Congo!");
            }
        }

        static internal void ManageShoppingCart(List<Product?> list, List<Product?> shoppingCart)
        {
            Console.WriteLine("A. Add new item to cart");
            Console.WriteLine("R. Read all items in the cart");
            Console.WriteLine("U. Update an item in the cart");
            Console.WriteLine("D. Delete item from the cart");
            Console.WriteLine("Q. Quit");

            char choice = 'Z';
            do
            {
                string? input = Console.ReadLine();
                if (input != null)
                {
                    choice = input[0];
                }

                switch (choice)
                {
                    case 'A':
                    case 'a':
                        Console.WriteLine("What is the name of the product?");
                        string? tempName = Console.ReadLine() ?? "ERROR";
                        bool duplicate = false;
                        foreach (Product? item in shoppingCart)
                        {
                            if (item != null && tempName == item.Name)
                            {
                                duplicate = true;
                            }
                        }

                        Product? tempProduct = null;

                        if (duplicate)
                        {
                            Console.WriteLine("There is already a product with that name in your cart.");
                        }
                        else if ((tempProduct = list.FirstOrDefault(p => p != null && p.Name == tempName)) == null)
                        {
                            Console.WriteLine("There is no product with that name in inventory.");
                        }
                        else
                        {
                            ShoppingCartServiceProxy.Current.AddOrUpdate(new Product
                            {
                                Name = tempProduct.Name,
                                Price = tempProduct.Price,
                                Quantity = 1
                            });
                            if (tempProduct.Quantity == 1)
                            {
                                ProductServiceProxy.Current.Delete(tempProduct.Id);
                            }
                            else
                            {
                                tempProduct.Quantity--;
                                ProductServiceProxy.Current.AddOrUpdate(tempProduct);
                            }
                        }
                        break;
                    case 'R':
                    case 'r':
                        shoppingCart.ForEach(Console.WriteLine);
                        break;
                    case 'U':
                    case 'u':
                        //select one product and replace with another
                        Console.WriteLine("Which product would you like to update?");
                        int selection = int.Parse(Console.ReadLine() ?? "-1");
                        var selectedProd = shoppingCart.FirstOrDefault(p => p != null && p.Id == selection);

                        if (selectedProd != null)
                        {
                            Console.WriteLine("I. Increase the quantity");
                            Console.WriteLine("D. Decrease the quantity");
                            Console.WriteLine("Anything Else. Back to menu");

                            char updateChoice = 'Z';
                            string? updateInput = Console.ReadLine();
                            if (updateInput != null)
                            {
                                updateChoice = updateInput[0];
                            }

                            switch (updateChoice)
                            {
                                case 'I':
                                case 'i':
                                    Console.WriteLine("How much would you like to add?");
                                    int? amount = int.Parse(Console.ReadLine() ?? "0");

                                    Product? tempInventory = null;
                                    if ((tempInventory = list.FirstOrDefault(p => p != null && p.Name == selectedProd.Name)) != null && tempInventory.Quantity >= amount)
                                    {
                                        selectedProd.Quantity = selectedProd.Quantity + amount;
                                        if (tempInventory.Quantity == amount)
                                        {
                                            ProductServiceProxy.Current.Delete(tempInventory.Id);
                                        }
                                        else
                                        {
                                            tempInventory.Quantity = tempInventory.Quantity - amount;
                                            ProductServiceProxy.Current.AddOrUpdate(tempInventory);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("You are trying to add more than is left in inventory.");
                                    }
                                    break;
                                case 'D':
                                case 'd':
                                    Console.WriteLine("How much would you like to delete?");
                                    amount = int.Parse(Console.ReadLine() ?? "0");

                                    tempInventory = list.FirstOrDefault(p => p != null && p.Name == selectedProd.Name);
                                    if (tempInventory != null)
                                    {
                                        if (selectedProd.Quantity <= amount)
                                        {
                                            tempInventory.Quantity = tempInventory.Quantity + selectedProd.Quantity;
                                            ProductServiceProxy.Current.AddOrUpdate(tempInventory);
                                            ShoppingCartServiceProxy.Current.Delete(selectedProd.Id);
                                        }
                                        else
                                        {
                                            tempInventory.Quantity = tempInventory.Quantity + amount;
                                            ProductServiceProxy.Current.AddOrUpdate(tempInventory);
                                            selectedProd.Quantity = selectedProd.Quantity - amount;
                                            ShoppingCartServiceProxy.Current.AddOrUpdate(selectedProd);
                                        }
                                    }
                                    else
                                    {
                                        if (selectedProd.Quantity <= amount)
                                        {
                                            ProductServiceProxy.Current.AddOrUpdate(new Product
                                            {
                                                Name = selectedProd.Name,
                                                Price = selectedProd.Price,
                                                Quantity = selectedProd.Quantity
                                            });
                                            ShoppingCartServiceProxy.Current.Delete(selectedProd.Id);
                                        }
                                        else
                                        {
                                            ProductServiceProxy.Current.AddOrUpdate(new Product
                                            {
                                                Name = selectedProd.Name,
                                                Price = selectedProd.Price,
                                                Quantity = amount
                                            });
                                            selectedProd.Quantity = selectedProd.Quantity - amount;
                                            ShoppingCartServiceProxy.Current.AddOrUpdate(selectedProd);
                                        }
                                    }

                                    break;
                                default:
                                    Console.WriteLine("Returning to the menu.");
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("That product doesn't exist");
                        }
                        break;
                    case 'D':
                    case 'd':
                        //select a product and delete it
                        Console.WriteLine("Which product would you like to delete?");
                        selection = int.Parse(Console.ReadLine() ?? "-1");
                        Product? tempProductInInv = null;

                        if ((tempProduct = shoppingCart.FirstOrDefault(p => p != null && p.Id == selection)) != null)
                        {
                            if ((tempProductInInv = list.FirstOrDefault(p => p != null && p.Name == tempProduct.Name)) == null)
                            {
                                ProductServiceProxy.Current.AddOrUpdate(new Product
                                {
                                    Name = tempProduct.Name,
                                    Price = tempProduct.Price,
                                    Quantity = tempProduct.Quantity
                                });
                            }
                            else
                            {
                                tempProductInInv.Quantity = tempProductInInv.Quantity + tempProduct.Quantity;
                                ProductServiceProxy.Current.AddOrUpdate(tempProductInInv);
                            }
                            ShoppingCartServiceProxy.Current.Delete(tempProduct.Id);
                        }
                        break;
                    case 'Q':
                    case 'q':
                        break;
                    default:
                        Console.WriteLine("Error: Unknown Command");
                        break;
                }
            } while (choice != 'Q' && choice != 'q');
        }




        static internal void ManageInventory(List<Product?> list)
        {
            Console.WriteLine("C. Create new inventory item");
            Console.WriteLine("R. Read all inventory items");
            Console.WriteLine("U. Update an inventory item");
            Console.WriteLine("D. Delete an inventory item");
            Console.WriteLine("Q. Quit");

            char choice = 'Z';
            do
            {
                string? input = Console.ReadLine();
                if (input != null)
                {
                    choice = input[0];
                }

                switch (choice)
                {
                    case 'C':
                    case 'c':
                        Console.WriteLine("What is the name of the product?");
                        string? tempName = Console.ReadLine() ?? "ERROR";
                        bool duplicate = false;
                        foreach (Product? item in list)
                        {
                            if (item != null && tempName == item.Name)
                            {
                                duplicate = true;
                            }
                        }

                        if (duplicate)
                        {
                            Console.WriteLine("There is already a product with that name. Consider updating the old product or choose a new name.");
                        }
                        else
                        {
                            Console.WriteLine("What is the price of the product?");
                            double? tempPrice = Math.Round(double.Parse(Console.ReadLine() ?? "-1"), 2); 
                            Console.WriteLine("What is the starting amount in stock?");
                            int? tempStock = int.Parse(Console.ReadLine() ?? "-1");
                            ProductServiceProxy.Current.AddOrUpdate(new Product
                            {
                                Name = tempName,
                                Price = tempPrice,
                                Quantity = tempStock
                            });
                        }
                        break;
                    case 'R':
                    case 'r':
                        list.ForEach(Console.WriteLine);
                        break;
                    case 'U':
                    case 'u':
                        //select one product and replace with another
                        Console.WriteLine("Which product would you like to update?");
                        int selection = int.Parse(Console.ReadLine() ?? "-1");
                        var selectedProd = list.FirstOrDefault(p => p.Id == selection);

                        if (selectedProd != null)
                        {
                            Console.WriteLine("N. Update the name");
                            Console.WriteLine("P. Update the price");
                            Console.WriteLine("S. Update the number of this product in stock");
                            Console.WriteLine("Anything Else. Back to menu");

                            char updateChoice = 'Z';
                            string? updateInput = Console.ReadLine();
                            if (updateInput != null)
                            {
                                updateChoice = updateInput[0];
                            }

                            switch (updateChoice)
                            {
                                case 'N':
                                case 'n':
                                    Console.WriteLine("What is the new name?");
                                    selectedProd.Name = Console.ReadLine() ?? "ERROR";
                                    ProductServiceProxy.Current.AddOrUpdate(selectedProd);
                                    break;
                                case 'P':
                                case 'p':
                                    Console.WriteLine("What is the new price?");
                                    selectedProd.Price = Math.Round(Convert.ToDouble(Console.ReadLine()), 2);
                                    ProductServiceProxy.Current.AddOrUpdate(selectedProd);
                                    break;
                                case 'S':
                                case 's':
                                    Console.WriteLine("What is the new amount in stock?");
                                    selectedProd.Quantity = Convert.ToInt32(Console.ReadLine());
                                    ProductServiceProxy.Current.AddOrUpdate(selectedProd);
                                    break;
                                default:
                                    Console.WriteLine("Returning to the menu.");
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("That product doesn't exist");
                        }
                        break;
                    case 'D':
                    case 'd':
                        //select a product and delete it
                        Console.WriteLine("Which product would you like to delete?");
                        selection = int.Parse(Console.ReadLine() ?? "-1");
                        ProductServiceProxy.Current.Delete(selection);
                        break;
                    case 'Q':
                    case 'q':
                        break;
                    default:
                        Console.WriteLine("Error: Unknown Command");
                        break;
                }
            } while (choice != 'Q' && choice != 'q');
        }
    }
}