import { useEffect, useState } from "react";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import BillPageCustomer from "./BillPageCustomer";
import BillPageSupplier from "./BillPageSupplier";
import { getData } from "../Common/FuncAxios";
import { useLayoutEffect } from "react";

const BillPage = () => {
  const [tabIndex, setTabIndex] = useState(0);
  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex);
  };

  const [listBank, setListBank] = useState([]);

  useLayoutEffect(() => {
    (async () => {
      let listBank = await getData(`Common/GetListBanks`);
      if (listBank && listBank.length > 0) {
        setListBank(listBank);
      }
    })();
  }, []);

  return (
    <>
      <Tabs
        selectedIndex={tabIndex}
        onSelect={(index) => HandleOnChangeTabs(index)}
      >
        <TabList>
          <Tab>Hóa Đơn KH</Tab>
          <Tab>Hóa Đơn NCC</Tab>
        </TabList>
        <TabPanel>
          {tabIndex === 0 && (
            <BillPageCustomer listBank={listBank}></BillPageCustomer>
          )}
        </TabPanel>
        <TabPanel>
          {tabIndex === 1 && (
            <BillPageSupplier listBank={listBank}></BillPageSupplier>
          )}
        </TabPanel>
      </Tabs>
    </>
  );
};

export default BillPage;
