import { useState } from "react";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import BillPageCustomer from "./BillPageCustomer";
import BillPageSupplier from "./BillPageSupplier";

const BillPage = () => {
  const [tabIndex, setTabIndex] = useState(0);
  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex);
  };

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
          {tabIndex === 0 && <BillPageCustomer></BillPageCustomer>}
        </TabPanel>
        <TabPanel>
          {tabIndex === 1 && <BillPageSupplier></BillPageSupplier>}
        </TabPanel>
      </Tabs>
    </>
  );
};

export default BillPage;
