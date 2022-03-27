# FishnChipsShop
Below is the description and written test cases to fulfil the below requirements
Introduction
This coding exercise is simple shopping basket to keep track of a customer’s purchases and their
total spend. A customer can add products to the basket and track the total cost.
Demonstrate all the good practices you would normally follow when writing high quality production
code.
There is no need to write a UI or command line interface. It is sufficient to demonstrate that your
application works by writing tests or invoking a ‘main’ method.
There is no need to use a database or persistent data store.
Submission
Submission of your solution can be either a link to a hosted git repository (e.g., github, gitlab), or by
emailing a zip file of a local git repository (the git commit history and the code files).

The Fish and Chip Shop Problem
Part 1
A customer can buy a portion of chips.
• A portion of chips costs £1.80
Part 2
A customer can buy a pie.
• A pie costs £3.20
• A pie has an expiry date
• A pie cannot be sold if it is past the expiry date
• A pie is sold at a discounted rate of 50% on the day of expiry only

Part 3
A customer can buy a pie and chips meal deal.
• A pie and chips meal deal applies a 20% discount to both items
• The discount can be applied to multiple meal deals
• The discount is not applied to items outside of a meal deal (for example, if there are 2 pies
and 3 portions of chips in the basket, only 2 pies and 2 portions of chips should be
discounted)
• Items in a meal deal are not eligible for any other discounts
• When multiple discounts may be applied, then the customer should always be offered the
lowest total price
