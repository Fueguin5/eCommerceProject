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
                    //ManageShoppingCart(list);
                }
                else if (invOrShop != 'Q' && invOrShop != 'q')
                {
                    Console.WriteLine("Invalid input. Try again.");
                }
            } while (invOrShop != 'Q' && invOrShop != 'q');
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
                            double? tempPrice = Math.Round(Convert.ToDouble(Console.ReadLine()), 2);    //add error handling for incorrect input later
                            Console.WriteLine("What is the starting amount in stock?");
                            int? tempStock = Convert.ToInt32(Console.ReadLine());   //add error handling for incorrect input later
                            ProductServiceProxy.Current.AddOrUpdate(new Product
                            {
                                Name = tempName,
                                Price = tempPrice,
                                NumInStock = tempStock
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
                                    selectedProd.NumInStock = Convert.ToInt32(Console.ReadLine());
                                    ProductServiceProxy.Current.AddOrUpdate(selectedProd);
                                    break;
                                default:
                                    Console.WriteLine("Returning to the menu.");
                                    break;
                            }
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